using UnityEditor;
using UnityEngine;

namespace Oloxo.Models.Editor {
    [CustomPropertyDrawer (typeof (Model))]
    public class ModelDrawer : PropertyDrawer {
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty (position, label, property);

            SerializedProperty idProp = property.FindPropertyRelative ("id");
            SerializedProperty meshProp = property.FindPropertyRelative ("mesh");

            if (idProp == null || meshProp == null) {
                EditorGUI.LabelField (position, "Error: Ensure fields are serializable.");
                return;
            }

            Rect idRect = new Rect (position.x, position.y, position.width * 0.4f, position.height);
            Rect meshRect = new Rect (position.x + position.width * 0.42f, position.y, position.width * 0.58f, position.height);

            EditorGUI.PropertyField (idRect, idProp, GUIContent.none);
            EditorGUI.PropertyField (meshRect, meshProp, GUIContent.none);

            EditorGUI.EndProperty ();
        }
    }
}