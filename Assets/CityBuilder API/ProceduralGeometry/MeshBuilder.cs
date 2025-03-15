using UnityEngine;
using System.Collections.Generic;


namespace Citybuilder.ProceduralGeometry {
    [RequireComponent (typeof (MeshFilter))]
    public class MeshBuilder {

        [System.NonSerialized] List<Vector3> vertices;
        [System.NonSerialized] List<Color> colors;
        [System.NonSerialized] List<int> triangles;
        [System.NonSerialized] List<Vector2> uvs;
        [System.NonSerialized] List<Vector3> normals;

        private Mesh mesh;

        public Mesh Mesh {
            get {
                return mesh;
            }
        }
        public MeshBuilder (string meshName) {
            mesh = new Mesh ();
            mesh.name = meshName;
            ClearMesh ();
        }
        public void PurgeColors () {
            colors.Clear ();
        }
        public void PurgeUVs () {
            uvs.Clear ();
        }
        public void PurgeNormals () {
            normals.Clear ();
        }
        public void ClearMesh () {
            mesh.Clear ();
            vertices = ListPool<Vector3>.Get ();
            colors = ListPool<Color>.Get ();
            uvs = ListPool<Vector2>.Get ();
            triangles = ListPool<int>.Get ();
            normals = ListPool<Vector3>.Get ();
        }
        public void BuildMesh (bool generateNormals) {
            mesh.SetVertices (vertices);
            ListPool<Vector3>.Add (vertices);

            mesh.SetColors (colors);
            ListPool<Color>.Add (colors);

            mesh.SetUVs (0, uvs);
            ListPool<Vector2>.Add (uvs);

            mesh.SetTriangles (triangles, 0);
            ListPool<int>.Add (triangles);

            if (generateNormals) {
                mesh.RecalculateNormals ();
            }
            else {
                mesh.SetNormals (normals);
                ListPool<Vector3>.Add (normals);
            }
        }
        public void AddMesh (Vector3 offset, float scale, float rotation, Mesh mesh) {
            if (mesh == null) return;
            int vertexCount = vertices.Count;

            //vector transformation
            foreach (Vector3 v in mesh.vertices) {

                if (rotation == 0 || rotation == 360) {
                    vertices.Add (offset + (v * scale));
                }
                else {
                    //rotate vector by quaternion
                    vertices.Add (offset + (RotatePointAroundPivot (v, Vector3.zero, new Vector3 (0, rotation, 0)) * scale));
                }
            }
            foreach (Vector2 uv in mesh.uv) uvs.Add (uv);
            foreach (int t in mesh.triangles) triangles.Add (vertexCount + t);
            foreach (Vector3 n in mesh.normals) {
                //normals.Add (n);

                if (rotation == 0 || rotation == 360) {
                    normals.Add (n);
                }
                else {
                    //rotate vector by quaternion
                    normals.Add (RotatePointAroundPivot (n, Vector3.zero, new Vector3 (0, rotation, 0)));
                }
            }
        }
        public void AddMeshWithColors (Vector3 offset, float scale, float rotation, Color color, Mesh mesh) {
            if (mesh == null) return;
            int vertexCount = vertices.Count;

            for (int i = 0 ; i < mesh.vertices.Length ; i++) colors.Add (color);
            //vector transformation
            foreach (Vector3 v in mesh.vertices) {

                if (rotation == 0 || rotation == 360) {
                    vertices.Add (offset + (v * scale));
                }
                else {
                    //rotate vector by quaternion
                    vertices.Add (offset + (RotatePointAroundPivot (v, Vector3.zero, new Vector3 (0, rotation, 0)) * scale));
                }
            }
            foreach (Vector2 uv in mesh.uv) uvs.Add (uv);
            foreach (int t in mesh.triangles) triangles.Add (vertexCount + t);
            foreach (Vector3 n in mesh.normals) {
                //normals.Add (n);

                if (rotation == 0 || rotation == 360) {
                    normals.Add (n);
                }
                else {
                    //rotate vector by quaternion
                    normals.Add (RotatePointAroundPivot (n, Vector3.zero, new Vector3 (0, rotation, 0)));
                }
            }
        }
        public void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
            int vertexIndex = vertices.Count;
            vertices.Add (v1);
            vertices.Add (v2);
            vertices.Add (v3);
            triangles.Add (vertexIndex);
            triangles.Add (vertexIndex + 1);
            triangles.Add (vertexIndex + 2);
        }
        public void AddTriangleColor (Color color) {
            colors.Add (color);
            colors.Add (color);
            colors.Add (color);
        }
        public void AddTriangleColor (Color c1, Color c2, Color c3) {
            colors.Add (c1);
            colors.Add (c2);
            colors.Add (c3);
        }
        public void AddTriangleUV (Vector2 uv1, Vector2 uv2, Vector2 uv3) {
            uvs.Add (uv1);
            uvs.Add (uv2);
            uvs.Add (uv3);
        }
        public void AddTriangleUV (Vector2 uv1) {
            uvs.Add (uv1);
            uvs.Add (uv1);
            uvs.Add (uv1);
        }
        public void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
            int vertexIndex = vertices.Count;
            vertices.Add (v1);
            vertices.Add (v2);
            vertices.Add (v3);
            vertices.Add (v4);
            triangles.Add (vertexIndex);
            triangles.Add (vertexIndex + 1);
            triangles.Add (vertexIndex + 2);
            triangles.Add (vertexIndex + 2);
            triangles.Add (vertexIndex + 3);
            triangles.Add (vertexIndex);
        }
        public void AddQuadColor (Color c1) {
            colors.Add (c1);
            colors.Add (c1);
            colors.Add (c1);
            colors.Add (c1);
        }
        public void AddQuadColor (Color c1, Color c2) {
            colors.Add (c1);
            colors.Add (c1);
            colors.Add (c2);
            colors.Add (c2);
        }
        public void AddQuadColor (Color c1, Color c2, Color c3, Color c4) {
            colors.Add (c1);
            colors.Add (c2);
            colors.Add (c3);
            colors.Add (c4);
        }
        public void AddQuadOnTile (Vector3 position, float scale = 1f) {
            AddQuad (
                position - (scale * new Vector3 (-.5f, 0, -.5f)),
                position - (scale * new Vector3 (-.5f, 0, .5f)),
                position - (scale * new Vector3 (.5f, 0, .5f)),
                position - (scale * new Vector3 (.5f, 0, -.5f))
                );
        }
        public void AddQuadUV (Vector2 uv1) {
            uvs.Add (uv1);
            uvs.Add (uv1);
            uvs.Add (uv1);
            uvs.Add (uv1);
        }
        public void AddQuadUV (Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4) {
            uvs.Add (uv1);
            uvs.Add (uv2);
            uvs.Add (uv3);
            uvs.Add (uv4);
        }
        public void AddQuadUV (float uMin, float uMax, float vMin, float vMax) {
            uvs.Add (new Vector2 (uMin, vMin));
            uvs.Add (new Vector2 (uMax, vMin));
            uvs.Add (new Vector2 (uMin, vMax));
            uvs.Add (new Vector2 (uMax, vMax));
        }
        public Vector3 RotatePointAroundPivot (Vector3 point, Vector3 pivot, Vector3 angles) {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler (angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }
        public Vector3[] RotatePointsAroundPivot (Vector3[] points, Vector3 pivot, Vector3 angles) {
            for (int i = 0 ; i < points.Length ; i++) {
                Vector3 dir = points[i] - pivot; // get point direction relative to pivot
                dir = Quaternion.Euler (angles) * dir; // rotate it
                points[i] = dir + pivot; // calculate rotated point
            }
            return points; // return it
        }
        public Vector3[] ExtrudePoints (int numPoints, Vector3 direction) {
            List<Vector3> points = new List<Vector3> (numPoints);
            for (int i = 0 ; i < numPoints ; i++) {
                points.Add (vertices[vertices.Count + (numPoints - 1)] + direction);
            }
            return points.ToArray ();
        }
    }
}