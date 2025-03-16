using UnityEngine;

namespace Oloxo.Models {

    [System.Serializable]
    /// <summary>
    /// Small data class representing information on models.
    /// </summary>
    public struct Model {

        [SerializeField] private Mesh mesh;
        [SerializeField] private ModelId id;

        public Model (ModelId id, Mesh mesh) {
            this.id = id;
            this.mesh = mesh;
        }

        public Mesh Mesh { get { return mesh; } }
        public ModelId Id { get { return id; } }
    }
}