using UnityEngine;
using System.Collections.Generic;
using Oloxo.Core;

namespace Oloxo.HexSystem {

    [RequireComponent (typeof (MeshFilter), typeof (MeshRenderer))]
    public class HexMesh : MonoBehaviour {

        //create static buffers for the triangulation
        static List<Vector3> vertices = new List<Vector3> ();
        static List<Vector2> uvs = new List<Vector2> ();
        static List<int> triangles = new List<int> ();

        private readonly Vector3 harvestOffset = new Vector3 (0, -.001f, 0);

        Mesh hexMesh;
        new MeshCollider collider;

        public void Awake () {
            GetComponent<MeshFilter> ().mesh = hexMesh = new Mesh ();
            collider = gameObject.gameObject.AddComponent<MeshCollider> ();

            hexMesh.name = "Hex Mesh";
            vertices = new List<Vector3> ();
            uvs = new List<Vector2> ();
            triangles = new List<int> ();
        }

        /// <summary>
        /// Triangulate an array of cells.
        /// </summary>
        /// <param name="cells"></param>
        public void Triangulate (HexCell[] cells) {
            //clear internal arrays

            hexMesh.Clear ();
            vertices.Clear ();
            uvs.Clear ();
            triangles.Clear ();

            //iterate over and triangulate each cell manually
            for (int i = 0 ; i < cells.Length ; i++) {
                Triangulate (cells[i]);
            }

            //set the data into the mesh
            hexMesh.vertices = vertices.ToArray ();
            hexMesh.uv = uvs.ToArray ();
            hexMesh.triangles = triangles.ToArray ();

            //push to the mesh collider
            collider.sharedMesh = hexMesh;
        }

        void Triangulate (HexCell cell) {
            //we deviate from the tutorial here because we do not use triangles in any way
            //instead, load the model and add its raw data into the mesh buffers

            var model = App.Current.Game.ModelLoader.GetModel (cell.Terrain.GetModelId());
            if (model.Mesh == null) return; //skip triangulation if there is no mesh to add

            Vector3 center = cell.transform.localPosition;
            Vector2 uvOffset = Vector2.zero;
            if (cell.Harvested) {
                center += harvestOffset;
                uvOffset = new Vector2 (.5f, 0);
            }

            //store the vertex offset for the triangle count
            int vertexStartIndex = vertices.Count;

            //use the model verts uvs and triangles offset to match the cells position
            // Adjust triangle indices to match the new vertex indices
            foreach (Vector3 v in model.Mesh.vertices) vertices.Add (v + center);
            foreach (Vector2 uv in model.Mesh.uv) uvs.Add (uv + uvOffset);
            foreach (int t in model.Mesh.triangles) triangles.Add (t + vertexStartIndex);
        }
    }
}