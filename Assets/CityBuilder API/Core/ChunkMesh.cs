using UnityEngine;
using System.Collections.Generic;
using Citybuilder.ProceduralGeometry;

namespace Citybuilder.Core {
    [RequireComponent (typeof (MeshFilter))]
    public class ChunkMesh : MonoBehaviour {

        MeshFilter meshFilter;
        MeshCollider meshCollider;
        MeshBuilder meshBuilder;

        public Bounds Bounds {
            get {
                if (TryGetComponent (out MeshRenderer renderer)) {
                    return renderer.bounds;
                }
                else return new Bounds ();
            }
        }

        public void Init () {
            meshBuilder = new MeshBuilder ("Chunk Mesh");
            meshFilter = GetComponent<MeshFilter> ();
            meshCollider = GetComponent<MeshCollider> ();
        }

        public void AddMesh (Vector3 offset, float scale, float rotation, Mesh mesh) {
            meshBuilder.AddMesh (offset, scale, rotation, mesh);
        }

        public void Clear () {
            meshBuilder.ClearMesh ();
        }
        public void Apply (bool generateNormals) {
            meshBuilder.BuildMesh (generateNormals);
            meshBuilder.PurgeColors (); //we don't need color data
            if (meshCollider) meshCollider.sharedMesh = meshBuilder.Mesh;
            if (meshFilter) meshFilter.sharedMesh = meshBuilder.Mesh;
        }
        public void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
            meshBuilder.AddQuad (v1, v2, v3, v4);
        }
        public void AddQuadColor (Color c1) {
            meshBuilder.AddQuadColor (c1);
        }
        public void AddQuadOnTile (Vector3 position, float scale = 1f) {
            meshBuilder.AddQuadOnTile (position, scale);
        }
        public void AddQuadUV (Vector2 uv1) {
            meshBuilder.AddQuadUV (uv1);
        }
    }
}