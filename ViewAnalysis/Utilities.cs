using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

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

        public Tuple<List<Point3d>,MeshFaceNormalList> getAnalysisLocations(Mesh mesh)
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

        
 

    }
}
