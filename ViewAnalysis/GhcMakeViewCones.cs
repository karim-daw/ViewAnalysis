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
          : base("MakeViewCones", "VA-MVC",
            "Make view cones for selected view points with a pre-determined resolution",
            "ViewAnalysis", "0_Preperation")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("AnalysisMesh", "Mesh", "Mesh representing object you want to perform view analysis on for each mesh face {item:Mesh}", GH_ParamAccess.item);
            pManager.AddNumberParameter("ViewAngle", "Angle", "Angle range. If None, 120 degrees is used {item:float}", GH_ParamAccess.item, 120.0);
            pManager.AddIntegerParameter("AngleIntervalResolution", "Interval", "Angle interval that will determine the resolution of the generated view cone {item:int}", GH_ParamAccess.item, 20);
            pManager.AddBooleanParameter("RunViewAnalysis", "Run", "Run the view analysis with the given input parameters {item:bool}", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ViewRays", "Rays", "A nested list containing a list or ray3ds for each analysis point {list[list],Ray3d}", GH_ParamAccess.list);
            pManager.AddNumberParameter("RayCountPerPoint", "RayCount", "A list of numbers referring to amount of hits each ray of each analysis point receives of the target mesh {list,int}", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.
            Mesh in_Mesh = new Rhino.Geometry.Mesh();
            double in_Angle = 0.0;
            int in_AngleStep = 0;
            Boolean in_Run = false;

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref in_Mesh))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No mesh provided");
                return;
            }
            if (!DA.GetData(1, ref in_Angle)) return;
            if (!DA.GetData(2, ref in_AngleStep)) return;
            if (!DA.GetData(3, ref in_Run)) return;

            // We should now validate the data and warn the user if invalid data is supplied.
            if (!in_Mesh.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "This Input is not valid, check if input is a mesh");
                return;
            }
            if (in_AngleStep >= in_Angle)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Interval input needs to be smaller than the Angle input");
                return;
            }
            if (in_AngleStep < 5)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "An interval smaller than 5 would cause a long and probably not diminishing return calculation ");
                return;
            }


            /////////// Main ///////////

            // 1. Get Analysis Points and Vectors
            Utilities utilities = new Utilities();
            Tuple<List<Point3d>, MeshFaceNormalList> tuple = utilities.GetAnalysisLocations(in_Mesh);

            // 2. Unpack Data and create cones
            List<Point3d> point3Ds = tuple.Item1;
            MeshFaceNormalList vector3Ds = tuple.Item2;

            // 3. Init ViewCone
            ViewCone firstViewCone = new ViewCone(point3Ds[0], vector3Ds[0], in_Angle, in_AngleStep);

            // 4. Calculate only first cone to get Ray count
            firstViewCone.ComputeViewCone();
            int out_RayCount = firstViewCone.RayCount;

            // 5. For each point, compute view cone
            List<List<Ray3d>> out_ViewRays = new List<List<Ray3d>>();
            for (int i = 0; i < point3Ds.Count; i++)
            {
                ViewCone viewCone = new ViewCone(point3Ds[i], vector3Ds[i], in_Angle, in_AngleStep);
                List<Ray3d> rays = viewCone.ComputeViewCone();
                out_ViewRays.Add(rays);
            }

            // 6. Finally assign the output parameters
            DA.SetData(0, out_ViewRays);
            DA.SetData(1, out_RayCount);
            
            
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