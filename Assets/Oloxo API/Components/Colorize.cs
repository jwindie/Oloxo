using UnityEngine;

namespace Oloxo.Components {

    [DisallowMultipleComponent]
    [RequireComponent (typeof (Renderer))]
    public class Colorize : MonoBehaviour {
        [SerializeField] Color color = Color.white;

        public Color Color {
            get {
                return color;
            }
        }

        /// <summary>
        /// Sets the color on a meshRenderer using a <see cref="MaterialPropertyBlock"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="color"></param>
        public static void SetColor (MeshRenderer target, Color color) {
            if (target) {
                MaterialPropertyBlock block = new MaterialPropertyBlock ();
                target.GetPropertyBlock (block);
                block.SetColor ("_Color", color);
                block.SetColor ("_BaseColor", color);
                target.SetPropertyBlock (block);
            }
        }

        private void Start () {
            SetColor (color);
        }

#if UNITY_EDITOR
        private void OnValidate () {
            SetColor (color);
        }
#endif

        public void SetColor (Color color) {
            this.color = color;
            var renderer = GetComponent<MeshRenderer> ();
            SetColor (renderer, color);
        }

        public void OnDestroy () {

            var renderer = GetComponent<MeshRenderer> ();
            if (renderer) {
                renderer.SetPropertyBlock (null);
            }
        }
    }
}