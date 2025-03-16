using UnityEngine;
using UnityEditor;

namespace Oloxo.HexSystem.Editor {

    [CustomPropertyDrawer (typeof (HexCoordinates))]
    public class HexCoordinatesDrawer : PropertyDrawer {

        //render the drawer
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            
            //extract the coordinate from the inspector properties
            HexCoordinates coordinates = new HexCoordinates (
                property.FindPropertyRelative ("x").intValue,
                property.FindPropertyRelative ("z").intValue
            );

            //create a prefix label 
            position = EditorGUI.PrefixLabel (position, label);

            //draw the coordinates as a formatted string
            GUI.Label (position, coordinates.ToString ());
        }
    }
}