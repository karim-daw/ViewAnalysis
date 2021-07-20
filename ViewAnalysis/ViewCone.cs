using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino;

namespace ViewAnalysis
{
    class ViewCone
    {
        public int RayCount = 0;

        public Point3d Point = new Point3d(0.0, 0.0, 0.0);
        public Vector3d Vector = new Vector3d(0.0, 1.0, 0.0);
        public double Angle = 120.0;
        public int AngleStep = 20;


        public ViewCone()
        {
        }


        public ViewCone(Point3d point, Vector3d vector, double angle, int angleStep)
        {
            Point = point;
            Vector = vector;
            Angle = angle;
            AngleStep = angleStep;
        }

        public List<Ray3d> ComputeViewCone()
        {
            // Create number range for cone
            int min = Convert.ToInt32(Math.Floor(Angle * -0.5));
            int max = Convert.ToInt32(Math.Ceiling(Angle * 0.5) + AngleStep);

            // move point straight forward
            Point3d movedPnt = Point + Vector;

            // init list for rays
            List<Ray3d> rays = new List<Ray3d>();

            // make counter
            int counter = 0;

            // rotate vector around z vector
            for(int i = min; i < max; i += AngleStep)
            {
                // convert first angle for horizontal fan to radians
                double angleR1 = (Math.PI / 180) * i;

                // disregard middle vector for now
                if (angleR1 == 0)
                {
                    continue;
                }
                // make a copy of vector
                Vector3d rotVector1 = new Rhino.Geometry.Vector3d(Vector);

                // rotate vector around z axis
                rotVector1.Rotate(angleR1, new Rhino.Geometry.Vector3d(0, 0, 1));

                // Rotate horizontal vector fan by 179 degrees
                for (int j = 0; j < 180; j += AngleStep)
                {
                    // convert second angle for vertical fan to radians
                    double angleR2 = (Math.PI / 180) * j;

                    // make a copy of vector
                    Vector3d rotVector2 = new Rhino.Geometry.Vector3d(rotVector1);

                    // rotate vector around center vector
                    rotVector2.Rotate(angleR2, Vector);

                    // create ray and append to list
                    Ray3d ray = new Rhino.Geometry.Ray3d(Point, rotVector2);
                    rays.Add(ray);
                    counter++;
                }
            }

            // Save to member fields
            RayCount = counter;

            return rays;

        }


        public List<Line> VisualiseViewCone(double amplitude, List<Ray3d> rays)
        {

            // init list of extracted directions from rays

            List<Line> lines = new List<Line>();

            for(int i = 0; i < rays.Count; i++)
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
