using UnityEngine;

[CreateAssetMenu (fileName = "New Mesh Asset", menuName = "Mesh/Mesh Asset", order = 0)]
public class MeshAsset : ScriptableObject {

    [SerializeField] private Mesh[] meshes;

    public int GetRandomIndex () {
        return Random.Range (0, meshes.Length);
    }

    public Mesh GetMesh (int MeshIndex) {
        return meshes[MeshIndex];
    }
}
