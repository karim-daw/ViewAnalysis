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
        /// Takes a mesh and returns the mesh face center points and their normals {item:Mesh}
        /// </summary>
        /// <param name="mesh">Input mesh to compute mesh face centers and normals from</param>
        /// <returns>Tuple with the mesh face center points and the normals {tuple:(Point3d,Vector3d)</returns>
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

        /// <summary>
        /// Generates a list of shuffled integers
        /// </summary>
        /// <param name="count">amount of random intergers</param>
        /// <returns> returns a list of shuffled integers</returns>
        public List<int> MakeRandomIntegers(int count)
        {
            // Init random
            // Happy Birthday mom
            var rand = new Random(3007);

            // Generate list of numbers
            var list = Enumerable.Range(0, count).ToList();

            // Shuffle list
            var randomized = list.OrderBy(item => rand.Next());

            List<int> shuffledList = new List<int>();
            foreach (var value in randomized)
            {
                shuffledList.Add(value);
            }
            return shuffledList;
        }

        // Thanks Andrew Heuman:)
        /// <summary>
        /// Converts a nested list into a data treee
        /// </summary>
        /// <typeparam name="T">Generic dataType</typeparam>
        /// <param name="list">nested list to convert to data tree</param>
        /// <returns>datatree equivalent of nested list</returns>
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
