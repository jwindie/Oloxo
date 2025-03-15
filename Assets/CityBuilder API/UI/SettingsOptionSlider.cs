using Citybuilder.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Citybuilder.UI {
    public class SettingsOptionSlider : SettingsOption {

        [Space]
        [SerializeField] private float minimum;
        [SerializeField] private float maximum;
        [Space]
        [SerializeField] private bool wholeNumbers;

        private Slider slider;
        protected override void Start () {

            slider = GetComponentInChildren<Slider> ();
            slider.wholeNumbers = wholeNumbers;
            slider.minValue = minimum;
            slider.maxValue = maximum;
            //Debug.Log ($"{setting}:{SettingsManager.Current.GetSetting (setting)}/{slider.value}");

            slider.onValueChanged.AddListener ((float f) => SettingsManager.Current.SetSetting (setting, f));
            RefreshValue ();
            SettingsManager.Current.presetWasLoaded += RefreshValue;

        }

        protected override void RefreshValue () {
            slider.SetValueWithoutNotify (SettingsManager.Current.GetSetting (setting));
        }
    }
}