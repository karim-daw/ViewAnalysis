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

        /// <summary>
        /// ViewCone Constructor
        /// </summary>
        public ViewCone()
        {
        }

        /// <summary>
        /// ViewCone Constructer
        /// </summary>
        /// <param name="point">View Point from which to generate view cone {item:Point3d}</param>
        /// <param name="vector">Starting Vector: this would be the normal vector of the meshface {item:Vector3d}</param>
        /// <param name="angle">View angle of the view cone</param>
        /// <param name="angleStep">The interval step size of the amount of vectors generated for view cone {item:int}</param>
        public ViewCone(Point3d point, Vector3d vector, double angle, int angleStep)
        {
            Point = point;
            Vector = vector;
            Angle = angle;
            AngleStep = angleStep;
        }

        /// <summary>
        /// Computes the View Cone 
        /// </summary>
        /// <returns>List of Ray3ds of the view cone {list:Ray3d}</returns>
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

            // loop through AngleStep and rotate vector around normal vector
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

                // if Vector is pointed up, rotate around x axis or y axis, doesnt matter
                if (rotVector1.Z == 1)
                {
                    rotVector1.Rotate(angleR1, new Rhino.Geometry.Vector3d(0, 1, 0));
                }
                else if (rotVector1.Z == -1)
                {
                    rotVector1.Rotate(angleR1, new Rhino.Geometry.Vector3d(0, 1, 0));
                }
                else
                {
                    // Compute the cross product
                    Vector3d cross = Rhino.Geometry.Vector3d.CrossProduct(rotVector1, new Rhino.Geometry.Vector3d(0, 0, 1));
                    rotVector1.Rotate(angleR1, cross);
                }

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

        public int ComputeHits(List<Ray3d> rays, Mesh targetMesh, Mesh obstaclesMesh)
        {
            int hit = 0;

            for(int i = 0; i < rays.Count; i++)
            {
                Ray3d ray = rays[i];

                double hitTarget = Rhino.Geometry.Intersect.Intersection.MeshRay(targetMesh, ray);
                double hitObstacle = Rhino.Geometry.Intersect.Intersection.MeshRay(obstaclesMesh, ray);

                if (hitTarget >= 0.0 && hitObstacle < 0.0) 
                {
                    hit++;
                }
            }
            return hit;

        }



    }
}
