﻿using System;
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

        public List<Ray3d> computeViewCone(Point3d point, Vector3d vector, double angle, int angleStep)
        {
            // Create number range for cone
            int min = Convert.ToInt32(Math.Floor(angle * -0.5));
            int max = Convert.ToInt32(Math.Ceiling(angle * 0.5) + angleStep);

            // move point straight forward
            Point3d movedPnt = point + vector;

            // init list for rays
            List<Ray3d> rays = new List<Ray3d>();

            // make counter
            int counter = 0;

            // rotate vector around z vector
            for(int i = min; i < max; i += angleStep)
            {
                // convert first angle for horizontal fan to radians
                double angleR1 = (Math.PI / 180) * i;

                // disregard middle vector for now
                if (angleR1 == 0)
                {
                    continue;
                }
                // make a copy of vector
                Vector3d rotVector1 = new Rhino.Geometry.Vector3d(vector);

                // rotate vector around z axis
                rotVector1.Rotate(angleR1, new Rhino.Geometry.Vector3d(0, 0, 1));

                // Rotate horizontal vector fan by 179 degrees
                for (int j = 0; j < 180; j += angleStep)
                {
                    // convert second angle for vertical fan to radians
                    double angleR2 = (Math.PI / 180) * j;

                    // make a copy of vector
                    Vector3d rotVector2 = new Rhino.Geometry.Vector3d(rotVector1);

                    // rotate vector around center vector
                    rotVector2.Rotate(angleR2, vector);

                    // create ray and append to list
                    Ray3d ray = new Rhino.Geometry.Ray3d(point, rotVector2);
                    rays.Add(ray);
                    counter++;
                }
            }
            RayCount = counter;

            return rays;


        }
    }
}
