using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace Oloxo.Components {
    public class ProcGenCard : MonoBehaviour {

        [SerializeField] private float width;
        [SerializeField] private float height;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private BoxCollider2D collider;
        [Space]
        [SerializeField] private string meshName;
        [SerializeField] private bool build;

        public float CornerRadius;
        public float[] CornerRadii = new float[4];
        public int CornerResolution;

        [Header ("Rounded Rect Options")]
        public bool UseSeparateCornerRadii;
        [SerializeField] private bool refresh;

        public List<Vector3> vertex = new List<Vector3> ();
        public List<Vector3> normal = new List<Vector3> ();
        public List<Vector2> uv = new List<Vector2> ();
        public List<int> triangle = new List<int> ();

        private Dictionary<Vector2, int> IndicesByVertex;

        public void ClearVertices () {
            vertex.Clear ();
            normal.Clear ();
            triangle.Clear ();
            uv.Clear ();
        }

        public void SetSize (float width, float height) {
            this.width = width;
            this.height = height;

            GenerateAndApplyMesh ();
        }

        public void OnValidate () {
            GenerateAndApplyMesh ();
            refresh = false;
        }

        void GenerateAndApplyMesh () {
            if (meshFilter) {
                GenerateMesh ();

                //add final triangle
                triangle.AddRange (new int[] { triangle[triangle.Count - 1], 0, 1 });


                Mesh m = new Mesh ();
                m.SetVertices (vertex);
                m.SetNormals (normal);
                m.SetTriangles (triangle, 0);
                m.SetUVs (0, uv);
                ClearVertices ();

    #if UNITY_EDITOR
                meshFilter.sharedMesh = m;
    #endif
            }

            if (collider) collider.size = new Vector2 (width, height);

            if (build) {
                SaveMeshToFile (meshFilter.sharedMesh);
                build = false;
            }
        }

        public void SaveMeshToFile (Mesh mesh) {
            if (mesh == null) return;

            StringBuilder sb = new StringBuilder ($"o {meshName}\n");
            sb.AppendLine ($"g {meshName}");


            // Write vertices
            foreach (Vector3 vertex in mesh.vertices) {
                sb.AppendLine ($"v {vertex.x} {vertex.y} 0");

            }

            // Write normals
            foreach (Vector3 normal in mesh.normals) {
                sb.AppendLine ($"vn {normal.x} {normal.y} {normal.z}");
            }

            // Write UVs
            foreach (Vector2 uv in mesh.uv) {
                sb.AppendLine ($"vt {1-uv.x} {uv.y}");
            }

            // Write triangles
            //for (int i = 0 ; i < mesh.triangles.Length ; i += 3) {
            //    sb.AppendLine ($"f {mesh.triangles[i] + 1} {mesh.triangles[i + 1] + 1} {mesh.triangles[i + 2] + 1}");
            for (int i = 0 ; i < mesh.triangles.Length ; i += 3) {
                sb.AppendLine (
                    $"f {mesh.triangles[i] + 1}/{mesh.triangles[i] + 1}/{mesh.triangles[i] + 1} " +
                    $"{mesh.triangles[i + 1] + 1}/{mesh.triangles[i + 1] + 1}/{mesh.triangles[i + 1] + 1} " +
                    $"{mesh.triangles[i + 2] + 1}/{mesh.triangles[i + 2] + 1}/{mesh.triangles[i + 2] + 1}"
                );
            }

            string path = Path.Combine (Application.dataPath, meshName + ".obj");
            File.WriteAllText (path, sb.ToString ());

            Debug.Log ($"Mesh saved to: {path}");

            System.Diagnostics.Process.Start (path);
        }

        public float GetMinCornerRadius () {
            float minLength = Mathf.Min (width, height) / 2f - 0.001f;
            return minLength;
        }

        public void GenerateMesh () {
            ClearVertices ();

            float minLength = Mathf.Min (width, height);
            float clampedCornerRadius = Mathf.Clamp (CornerRadius, 0.001f, minLength / 2f - 0.001f);
            float[] clampedCornerRadii = new float[] {
                Mathf.Clamp (CornerRadii[0], 0.001f, minLength / 2f - 0.001f),
                Mathf.Clamp (CornerRadii[1], 0.001f, minLength / 2f - 0.001f),
                Mathf.Clamp (CornerRadii[2], 0.001f, minLength / 2f - 0.001f),
                Mathf.Clamp (CornerRadii[3], 0.001f, minLength / 2f - 0.001f),
            };

            Vector2[] interiorCorners = new Vector2[4];

            float xOffset = (width / 2f - clampedCornerRadius);
            float yOffset = (height / 2f - clampedCornerRadius);

            interiorCorners[0] = new Vector2 (xOffset, yOffset);
            interiorCorners[1] = new Vector2 (-xOffset, yOffset);
            interiorCorners[2] = new Vector2 (-xOffset, -yOffset);
            interiorCorners[3] = new Vector2 (xOffset, -yOffset);

            if (UseSeparateCornerRadii) {
                xOffset = width / 2f;
                yOffset = height / 2f;
                interiorCorners[0] = new Vector2 ((xOffset - clampedCornerRadii[0]), (yOffset - clampedCornerRadii[0]));
                interiorCorners[1] = new Vector2 (-(xOffset - clampedCornerRadii[1]), (yOffset - clampedCornerRadii[1]));
                interiorCorners[2] = new Vector2 (-(xOffset - clampedCornerRadii[2]), -(yOffset - clampedCornerRadii[2]));
                interiorCorners[3] = new Vector2 ((xOffset - clampedCornerRadii[3]), -(yOffset - clampedCornerRadii[3]));
            }

            // Add central vert
            vertex.Add (Vector2.zero);
            normal.Add (Vector3.forward);
            uv.Add (new Vector2 (.5f, .5f));

            // Generate exterior vertices
            List<Vector2> exteriorVerts = new List<Vector2> ();
            for (int c = 0 ; c < 4 ; c++) {
                float radius = clampedCornerRadius;
                if (UseSeparateCornerRadii) {
                    radius = clampedCornerRadii[c];
                }

                exteriorVerts.AddRange (GenerateCornerVerts (interiorCorners[c], radius, 90f * c, CornerResolution));
            }

            int n = exteriorVerts.Count + 1;

            // Add verts and triangles to stream
            for (int i = 0 ; i < exteriorVerts.Count ; i++) {
                vertex.Add (exteriorVerts[i]);
                normal.Add (Vector3.forward);
                uv.Add (new Vector2 (0.5f * exteriorVerts[i].x / (width * 0.5f) + 0.5f, 0.5f * exteriorVerts[i].y / (height * 0.5f) + 0.5f));
                Debug.Log (uv[uv.Count - 1]);
                //vh.AddVert (vert);

                triangle.AddRange (new int[] { 0, (i + 2) % n, (i + 1) % n });
            }
        }

        void BuildCorner (Vector3 point, float startAngle, int currentCornerIdx, Vector2[] interiorCorners, Vector2 startVertex, Vector2 endVertex) {
            List<Vector2> cornerVerts = new List<Vector2> ();

            int startIndex = IndicesByVertex.Count;

            for (int i = 0 ; i < CornerResolution ; i++) {
                float theta = Mathf.Deg2Rad * 90f * (float) (i + 1) / (CornerResolution + 1) + Mathf.Deg2Rad * startAngle;
                float x = Mathf.Cos (theta) * CornerRadius;
                float y = Mathf.Sin (theta) * CornerRadius;
                Vector2 pos = interiorCorners[currentCornerIdx] + new Vector2 (x, y);
                cornerVerts.Add (pos);
                IndicesByVertex.Add (pos, startIndex + i);
            }

            // Add verts to stream
            foreach (Vector2 v in cornerVerts) {
                point = v;

                //Vector2 uv = v;
                //uv.x = uv.x / (width / 2f) + 0.5f;
                //uv.x = uv.y / (height / 2f) + 0.5f;
                //point.uv0 = uv;

                vertex.Add (point);
                normal.Add (Vector3.forward);
            }

            cornerVerts.Insert (0, startVertex);
            cornerVerts.Add (endVertex);

            for (int i = 0 ; i < cornerVerts.Count - 1 ; i++) {
                triangle.AddRange (new int[] { currentCornerIdx, IndicesByVertex[cornerVerts[i]], IndicesByVertex[cornerVerts[i + 1]] });
            }
        }

        List<Vector2> GenerateCornerVerts (Vector2 center, float radius, float startAngle, float resolution) {
            List<Vector2> verts = new List<Vector2> ();
            resolution = resolution <= 0 ? 1 : resolution;

            for (int i = 0 ; i <= resolution ; i++) {
                float theta = Mathf.Deg2Rad * (90f * (float) i / resolution + startAngle);
                float x = Mathf.Cos (theta) * radius;
                float y = Mathf.Sin (theta) * radius;

                Vector2 v = center + new Vector2 (x, y);
                verts.Add (v);
            }

            return verts;
        }

        public void SetCornerRadii (float[] cornerRadii) {
            CornerRadii = cornerRadii;
        }

        public void Refresh () {
    #if UNITY_EDITOR
            OnValidate ();
    #endif
        }
    }
}