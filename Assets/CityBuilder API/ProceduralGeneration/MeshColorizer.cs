using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent (typeof (MeshRenderer))]

public class MeshColorizer : MonoBehaviour {
    public Color color = Color.white;

    private void Start () {
        SetColor (color);
    }

    private void OnDisable () {
        Clear ();
    }
    private void OnEnable () {
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
        if (renderer) {
            MaterialPropertyBlock block = new MaterialPropertyBlock ();
            renderer.GetPropertyBlock (block);
            block.SetColor ("_Color", color);
            block.SetColor ("_BaseColor", color);
            block.SetColor ("_MainColor", color);
            renderer.SetPropertyBlock (block);
        }
    }

    public void Clear () {
        var renderer = GetComponent<MeshRenderer> ();
        if (renderer) {
            renderer.SetPropertyBlock (null);
        }
    }


    public void OnDestroy () {

        var renderer = GetComponent<MeshRenderer> ();
        if (renderer) {
            renderer.SetPropertyBlock (null);
        }
    }
}
