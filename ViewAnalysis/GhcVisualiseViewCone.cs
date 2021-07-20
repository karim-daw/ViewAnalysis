using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;


namespace ViewAnalysis
{
    public class GhcVisualiseViewCone : GH_Component
    {

        public GhcVisualiseViewCone()
          : base("VisualiseViewCone", "VA-VVC",
              "Visualises a pre-determined percentage of all the view cones that were made for the analysis mesh",
              "ViewAnalysis", "2_Visualize")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("RaysToVisualize", "Rays", "A list of nested lists of rays to visualize with lines {List[List]:Ray3d}", GH_ParamAccess.list);
            pManager.AddNumberParameter("VectorAmplitude", "Amplitude", "Amplitude of vector this would be equivalent to the length of the lines representating the rays to visualize {item:float}", GH_ParamAccess.item, 10.0);
            pManager.AddNumberParameter("Percentage", "Percentage", "Percentage (number between 0 and 1) of view cones to visualize on the analysis mesh {item:float}", GH_ParamAccess.item, 0.1);
            pManager.AddBooleanParameter("RunVisualizer", "Run", "Run the visualizer with the given input parameters {item:bool}", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("ViewLines", "Lines", "A Percentage of the ViewCones visualized as lines {list,Line}", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.
            List<List<Ray3d>> in_Rays = new List<List<Ray3d>>();
            double in_Amplitude = 0.0;
            double in_Percentage = 0.0;
            Boolean in_Run = false;

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            
            if (!DA.GetDataList(0, in_Rays))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No rays provided");
                return;
            }
            if (!DA.GetData(1, ref in_Amplitude)) return;
            if (!DA.GetData(2, ref in_Percentage)) return;
            if (!DA.GetData(3, ref in_Run)) return;

            // We should now validate the data and warn the user if invalid data is supplied.
            if (in_Amplitude < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The amplitude should not be 0 or a negative number");
                return;
            }
            if (in_Percentage < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The percentage has to be a positive number between 0 and 1, you inputed a negative number");
                return;
            }
            if (in_Percentage > 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The percentage has to be a positive number between 0 and 1, you inputed a number greater than 1");
                return;
            }

            /////////// Main ///////////

            // 1. Get Nested Rays list length and compute Reduced list length from percentage input
            int raysCount = in_Rays.Count;
            int reducedRayCount = Convert.ToInt32(raysCount * in_Percentage);

            // 2. Get list of random numbers
            Utilities utilities = new Utilities();
            List<int> randInts = utilities.MakeRandomIntegers(0, raysCount-1, reducedRayCount);

            // 3. Get nested List of Rays and loop through them with random indexes

            // init nested list of lines
            List<List<Line>> nlines = new List<List<Line>>();
            for (int i = 0; i < reducedRayCount; i++)
            {
                // Get random index as iterator
                int rI = randInts[i];

                // Get list of rays from nested list using random int iterator
                List<Ray3d> rays = in_Rays[rI];

                // Compute lines and append
                List<Line> lines = new ViewCone().VisualiseViewCone(in_Amplitude, rays);
                nlines.Add(lines);
            }

            // 4. Convert nested list to Data Tree
            DataTree<Line> out_DataTree = utilities.ListOfListsToTree(nlines);

            // 5. Finally assign the output parameters
            DA.SetDataTree(0, out_DataTree);


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
            get { return new Guid("FDC62C6D-7C03-412D-8FF8-B76439197730"); }
        }
    }
}