using UnityEngine;
using UnityEngine.UI;

namespace Citybuilder.Testing {
    public class MapEditorOption : MonoBehaviour {
        public Toggle enable;
        public Toggle state;
        public Text label;

        private void Start () {
            label.text = name;
            UpdateSetting ();
        }

        public void UpdateSetting () {
            MapEditor.Current.UpdateSetting (name.ToUpper (), enable ? enable.isOn : false, state ? state.isOn : false);
        }
    }
}