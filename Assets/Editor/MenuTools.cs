using UnityEngine;
using System.IO;
using UnityEditor;

public static class MenuTools {
    [MenuItem ("Tools/Remove MTL")]
    public static void RemoveAllMTLFiles () {
        foreach (string f in Directory.GetFiles (Application.dataPath, "*.mtl", SearchOption.AllDirectories)) {
            File.Delete (f);
        }
        foreach (string f in Directory.GetFiles (Application.dataPath, "*.mtl.meta", SearchOption.AllDirectories)) {
            File.Delete (f);
        }
    }
}
