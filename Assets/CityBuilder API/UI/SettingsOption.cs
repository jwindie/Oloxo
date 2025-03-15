using Citybuilder.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Citybuilder.UI {
    public class SettingsOption : MonoBehaviour {

        [SerializeField] protected string setting;
        [SerializeField] protected string displayName;

        private Toggle toggle;

        protected virtual void Start () {

            toggle = GetComponentInChildren<Toggle> ();
            RefreshValue ();
            if (toggle) toggle.onValueChanged.AddListener ((bool state) => SettingsManager.Current.SetSetting (setting, state ? 1 : 0));
            SettingsManager.Current.presetWasLoaded += RefreshValue;
        }

        protected virtual void RefreshValue () {
            toggle.SetIsOnWithoutNotify (SettingsManager.Current.GetSetting (setting) == 1);
        }


        protected void OnValidate () {

            var textMesh = GetComponentInChildren<TMPro.TextMeshProUGUI> ();
            if (textMesh) {

                if (displayName == "") {
                    textMesh.SetText (name = "Some Option");
                }
                else {
                    textMesh.SetText (name = displayName);
                }
            }
        }
    }
}