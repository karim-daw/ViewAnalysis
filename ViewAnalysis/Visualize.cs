using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace ViewAnalysis
{
    class Visualize
    {

        /// <summary>
        /// Visualize Cosntructor
        /// </summary>
        public Visualize()
        {
        }

        /// <summary>
        /// Visualises generated view cone as a set of lines
        /// </summary>
        /// <param name="amplitude">The size of the vector: aka length of line {item:doube}</param>
        /// <param name="rays">List of rays to visualize {list:Ray3d}</param>
        /// <returns> Returns a list of lines representing the view cone {list:Line}</returns>
        public List<Line> VisualiseViewCone(double amplitude, List<Ray3d> rays)
        {

            // init list of extracted directions from rays

            List<Line> lines = new List<Line>();

            for (int i = 0; i < rays.Count; i++)
            {
                Ray3d ray = rays[i];

                // Get Ray Position
                Point3d pos = ray.Position;

                // Get Ray Vector and add to list
                Vector3d dir = ray.Direction;

                // Move pos by vector by amplitude provided
                Point3d mPos = pos + (dir * amplitude);

                // Create Line
                Line line = new Line(pos, mPos);

                // output vectors as lines
                lines.Add(line);
            }

            return lines;

        }

    }
}
