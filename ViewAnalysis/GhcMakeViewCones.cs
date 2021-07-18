using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Rhino.Geometry.Collections;

namespace ViewAnalysis
{
    public class GhcMakeViewCones : GH_Component
    {
        public GhcMakeViewCones()
          : base("GenerateViewCones", "VA-GVC",
            "Generate view cones for selected view points with a pre-determined resolution",
            "ViewAnalysis", "0_Preperation")
        {
        }
        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("AnalysisMesh", "Mesh", "Mesh representing object you want to perform view analysis on for each mesh face {item:mesh}", GH_ParamAccess.item);
            pManager.AddNumberParameter("ViewAngle", "Angle", "Angle range. If None, 120 degrees is used {item:float}", GH_ParamAccess.item, 120);
            pManager.AddNumberParameter("AngleIntervalResolution", "Interval", "Angle interval that will determine the resolution of the generated view cone {item:int}", GH_ParamAccess.item, 20);
            pManager.AddBooleanParameter("RunViewAnalysis", "Run", "Run the view analysis with the given input parameters {item:bool}", GH_ParamAccess.item, false);
        }
        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ViewRays", "Rays", "A nested list containing a list or ray3ds for each analysis point {list[list],ray3d}", GH_ParamAccess.list);
            pManager.AddNumberParameter("RayCountPerPoint", "RayCount", "A list of numbers referring to amount of hits each ray of each analysis point receives of the target mesh {list,int}", GH_ParamAccess.list);
        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.
            Mesh mesh = new Rhino.Geometry.Mesh();
            double angle = 0.0;
            int interval = 0;
            Boolean run = false;

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref mesh))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No mesh provided");
                return;
            }
            if (!DA.GetData(1, ref angle)) return;
            if (!DA.GetData(2, ref interval)) return;
            if (!DA.GetData(3, ref run)) return;

            // We should now validate the data and warn the user if invalid data is supplied.
            if (!mesh.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "This Input is not valid, check if input is a mesh");
                return;
            }
            if (interval >= angle)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Interval input needs to be smaller than the Angle input");
                return;
            }
            if (interval < 5)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "An interval smaller than 5 would cause a long and probably not diminishing return calculation ");
                return;
            }


            // Main
            // 1. Get Analysis Points and Vectors
            Utilities utilities = new Utilities();
            Tuple<List<Point3d>, MeshFaceNormalList> tuple = utilities.getAnalysisLocations(mesh);

            // 2. Unpack Data and create cones
            List<Point3d> point3Ds = tuple.Item1;
            MeshFaceNormalList vector3Ds = tuple.Item2;

            // 3. For each point, compute view cone
            ViewCone viewCone = new ViewCone();
            List<List<Ray3d>> viewRays = new List<List<Ray3d>>();
            for (int i = 0; i < point3Ds.Count; i++)
            {
                Point3d pnt = point3Ds[i];
                Vector3d vec = vector3Ds[i];
                List<Ray3d> rays = viewCone.computeViewCone(pnt, vec, angle, interval);
                viewRays.Add(rays);
            }

            DA.SetData(0, viewRays);
            // Finally assign the spiral to the output parameter.
            
        }


        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("8A32A25E-110D-4EB5-9D07-A98A76DA6B5E");
    }
}