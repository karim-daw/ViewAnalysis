using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace ViewAnalysis
{
    public class ViewAnalysisInfo : GH_AssemblyInfo
    {
        public override string Name => "ViewAnalysis";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("7CD0BDC2-5C8F-4092-89CD-33F436FF067F");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}