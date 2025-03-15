using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Core {

    /// <summary>
    /// Dictionary containing all 3D meshes used in game
    /// Avoids the file size overhead of prefabs and the resources.load
    /// </summary>
    public static class MeshLibrary {
        private static Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh> ();

        public static Mesh GetMesh (string name) {
            if (meshes.TryGetValue (name, out Mesh value)) {
                return value;
            }
            else return null;
        }

        public static void AddMesh (string name, Mesh mesh) {

            //prevent overwrites of existing meshes
            if (meshes.ContainsKey (name)) {
                Debug.LogWarning ($"{name} was not added to the library as there was already an entry for {name}");
                return;
            }

            meshes.Add (name, mesh);
        }

        public static void AddMeshesFromResources (string path) {
            foreach (GameObject g in Resources.LoadAll<GameObject> (path)) {

                //get all meshFilters in the object
                foreach (MeshFilter mf in g.GetComponentsInChildren<MeshFilter> ()) {
                    AddMesh ($"{path}/{mf.name}", mf.sharedMesh);
                }
            }
        }

        public static void LogMeshes () {
            Debug.Log ($"Mesh Library Entries: {meshes.Keys.Count}");

            foreach (string name in meshes.Keys) {
                Debug.Log ($"Mesh Library Entry: {name}");
            }
        }
        public static void LogMeshes (JWIndie.Utilities.FileLog fileLog) {

            foreach (string name in meshes.Keys) {
                fileLog.WriteLine ($"\t{name}", false);
            }
        }
    }
}
