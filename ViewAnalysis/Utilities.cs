using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace ViewAnalysis
{
    class Utilities
    {
        /// <summary>
        /// Visits all mesh faces and gets centers points and normals
        /// <summary>
        /// Input:
        ///     Analysis mesh { Mesh: Item }
        /// Output:
        ///     Center points and normals {Point3d,MeshFaceNormalList : Tuple( list(Point3d) , MeshFaceNormalList ) }

        public Tuple<List<Point3d>,MeshFaceNormalList> GetAnalysisLocations(Mesh mesh)
        {
            if (mesh.FaceNormals.Count == 0)
            {
                mesh.FaceNormals.ComputeFaceNormals();
            }

            MeshFaceList faces = mesh.Faces;
            MeshFaceNormalList normals = mesh.FaceNormals;
            List<Point3d> centers = new List<Point3d>();

            int fCount = faces.Count;

            for(int i = 0; i < faces.Count; i++)
            {
                Point3d cPnt = faces.GetFaceCenter(i);
                Vector3d norm = normals[i];
                centers.Add(cPnt);
            }

            return Tuple.Create(centers, normals);

        }

        public List<int> MakeRandomIntegers(int min, int max, int count)
        {
            // Init random
            var rand = new Random(3007);

            // Generate list of random integers of length "count" between "start" and "end
            List<int> randNums = new List<int>();

            for (int i = 0; i <= count; i++)
            {
                int randNum = rand.Next(min, max + 1);
                randNums.Add(randNum);
            }
            return randNums;
        }


        // Thanks Andrew Heuman:)
        public DataTree<T> ListOfListsToTree<T>(List<List<T>> list)
        {
            DataTree<T> tree = new DataTree<T>();
            int i = 0;
            foreach (List<T> innerList in list)
            {
                tree.AddRange(innerList, new GH_Path(new int[] { 0, i }));
                i++;
            }
            return tree;
        }



    }
}
