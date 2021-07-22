using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace ViewAnalysis
{
    public class GhcComputeTargetHits : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
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
            pManager.AddGenericParameter("RaysToCompute", "Rays", "A list of nested lists of rays to compute hit occurances of target mesh {List[List]:Ray3d}", GH_ParamAccess.list); pManager.AddNumberParameter("ViewAngle", "Angle", "Angle range. If None, 120 degrees is used {item:float}", GH_ParamAccess.item, 120.0);
            pManager.AddMeshParameter("TargetMesh", "Target", "Mesh to be used as a target for View Ray calculation { item: mesh}", GH_ParamAccess.item);
            pManager.AddMeshParameter("ObstaclesMesh", "Obstacles", "Mesh to be used as Obstacles occluding the view of the target(dont forget self occluding objects) { item: mesh}", GH_ParamAccess.item);
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