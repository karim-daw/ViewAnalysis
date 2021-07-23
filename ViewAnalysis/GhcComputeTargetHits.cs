using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ViewAnalysis
{
    public class GhcComputeTargetHits : GH_Component
    {
        public GhcComputeTargetHits()
          : base("ComputeTargetHits", "VA-CTH",
              "Computes the amount of rays that hit the target mesh from every view point",
              "ViewAnalysis", "1_Compute")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("RaysToCompute", "Rays", "A list of nested lists of rays to compute hit occurances of target mesh {List[List]:Ray3d}", GH_ParamAccess.list);
            pManager.AddMeshParameter("TargetMesh", "Target", "Mesh to be used as a target for View Ray calculation { item: mesh}", GH_ParamAccess.item);
            pManager.AddMeshParameter("ObstaclesMesh", "Obstacles", "Mesh to be used as Obstacles occluding the view of the target(dont forget self occluding objects) { item: mesh}", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddBooleanParameter("RunComputeTargetHits", "Run", "Run the view analysis and compute how many rays hit the target mesh {item:bool}", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("TargetHits", "Hits", "The amount of hits each view point recieved for each of its view rays {list:int}", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<List<Ray3d>> in_Rays = new List<List<Ray3d>>();
            Mesh in_TarMesh = new Mesh();
            Mesh in_ObsMesh = new Mesh();
            Boolean in_Run = false;

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.

            if (!DA.GetDataList(0, in_Rays))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No rays provided");
                return;
            }

            if (!DA.GetData(1, ref in_TarMesh))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No target mesh provided");
                return;
            }


            if (DA.GetData(3, ref in_Run) == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Set 'Run' to true in order to compute rays");
                return;
            }

            /////////// Main ///////////

            // 1. start counter per view point
            List<int> out_viewHits = new List<int>();

            // 2. check if run is on, if not, return
            if (in_Run == true)
            {
                // 3. loop through nested list of rays
                for (int i = 0; i < in_Rays.Count; i++)
                {
                    // Access sub list of rays
                    List<Ray3d> rays = in_Rays[i];

                    // 4. init view cone to call ComputeRayHits method
                    ViewCone viewCone = new ViewCone();

                    int hits = 0;

                    if (!DA.GetData(2, ref in_ObsMesh))
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "No obstacle mesh provided, calculation will be performed without obstacles");
                        hits = viewCone.ComputeRayHits(rays, in_TarMesh);
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Calculation will be performed with obstacles");
                        hits = viewCone.ComputeRayHits(rays, in_TarMesh, in_ObsMesh);
                    }
                    
                    // 5. Add hits numbers to viewHits List
                    out_viewHits.Add(hits);
                }
            }

            // 6. Finally assign the output parameters
            DA.SetDataList(0, out_viewHits);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("86FA4140-4275-4147-891B-90B42C8D1913"); }
        }
    }
}