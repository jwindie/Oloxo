using Citybuilder.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Citybuilder.Core {
    public class SettingsManager : Singleton<SettingsManager> {

        public const float MIN_FOG_FAR_DISTANCE = 30;
        public const float MAX_FOG_FAR_DISTANCE = 200;


        public static readonly string[] SETTINGS_NAMES = new string[]{
            "LIGHT_REALTIME",
            "LIGHT_INTENSITY",
            "LIGHT_LATTITUDE",
            "LIGHT_LONGITUDE",
            "SHADOW_ENABLE",
            "SHADOW_INTENSITY",
            "SHADOW_RESOLUTION",
            "CAMERA_NEAR_SPEED",
            "CAMERA_FAR_SPEED",
            "FOG_DISTANCE",
            "FOG_ENABLE",
        };

        public static readonly float[] DefaultSettings = new float[] {
            .5f, //LIGHT_REALTIME
            0.731877f, //LIGHT_INTENSITY
            46.079f, //LIGHT_LATTITUDE
            145.355f, //LIGHT_LONGITUDE
            1, //SHADOW_ENABLE
            .5f, //SHADOW_INTENSITY
            3, //SHADOW_RESOLUTION
            10, //CAMERA_NEAR_SPEED
            50, //CAMERA_FAR_SPEED
            0.8659389f, //FOG_DISTANCE
            1, //FOG_ENABLE
        };

        [SerializeField] private bool enableUIBlocker;
        [Space (10)]
        [SerializeField] private Material mainMaterial;
        [SerializeField] private Light directionalLight;
        [SerializeField] private UniversalRenderPipelineAsset urpAsset;
        [SerializeField] private Volume globalVolume;
        [SerializeField] private ForwardRendererData forwardRendererData;
        [SerializeField] private Button applyButton;
        [SerializeField] private GameObject settings;


        private Transform longitude;
        private bool visible;

        public System.Action presetWasLoaded;

        public float[] GetSettings {
            get {
                float[] _ = new float[SETTINGS_NAMES.Length];
                for (int i = 0 ; i < SETTINGS_NAMES.Length ; i++) {

                    _[i] = GetSetting (SETTINGS_NAMES[i]);
                    //Debug.Log ($"GET {SETTINGS_NAMES[i]}: {_[i]}");
                }
                return _;
            }
        }

        public Volume Volume {
            get {
                return globalVolume;
            }
        }

        public void SetSetting (string setting, float value) {

            //Debug.Log ($"SET {setting}: {value}");
            //settings are written like shader directives
            switch (setting) {
                case "LIGHT_REALTIME":
                    //value is 10x greater
                    mainMaterial.SetFloat ("_EmissionFactor", value);
                    directionalLight.enabled = value != 1;
                    break;

                case "LIGHT_INTENSITY":
                    //value is 10x greater
                    directionalLight.intensity = value;
                    break;

                case "LIGHT_LATTITUDE":
                    directionalLight.transform.localRotation = Quaternion.Euler (value, 0, 0);
                    break;

                case "LIGHT_LONGITUDE":
                    longitude.localRotation = Quaternion.Euler (0, value, 0);
                    break;

                case "SHADOW_ENABLE":
                    UnityGraphicsBullshit.MainLightCastShadows = value == 1;
                    break;

                case "SHADOW_RESOLUTION":
                    int resolution = (int) value;
                    if (resolution == 0) UnityGraphicsBullshit.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._256;
                    else if (resolution == 1) UnityGraphicsBullshit.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._512;
                    else if (resolution == 2) UnityGraphicsBullshit.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._1024;
                    else if (resolution == 3) UnityGraphicsBullshit.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._2048;
                    else if (resolution == 4) UnityGraphicsBullshit.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._4096;
                    break;

                case "SHADOW_INTENSITY":
                    directionalLight.shadowStrength = value;
                    break;

                case "CAMERA_NEAR_SPEED":
                    Core.CameraController.Current.SetMoveSpeedValues (new Vector2 (value, Core.CameraController.Current.MoveSpeedValues.y));
                    break;

                case "CAMERA_FAR_SPEED":
                    Core.CameraController.Current.SetMoveSpeedValues (new Vector2 (Core.CameraController.Current.MoveSpeedValues.x, value));
                    break;

                case "FOG_DISTANCE":
                    Core.CameraController.Current.SetFarClip (RenderSettings.fogEndDistance = Mathf.Lerp (MIN_FOG_FAR_DISTANCE, MAX_FOG_FAR_DISTANCE, value));
                    break;

                case "FOG_ENABLE":
                    RenderSettings.fog = value == 1;
                    break;

                //case "FX_SSAO":
                //    for (int x = 0 ; x < forwardRendererData.rendererFeatures.Count ; x++) {
                //        if (forwardRendererData.rendererFeatures[x].name == "SSAO") {
                //            forwardRendererData.rendererFeatures[x].SetActive (value == 1);
                //        }
                //    }
                //    break;

                //case "FX_MOTIONBLUR":
                //    if (globalVolume.profile.TryGet (out MotionBlur motionBlur)) {
                //        motionBlur.intensity.value = value;
                //        motionBlur.active = value != 0;
                //    }
                //    break;

                default:
                    Debug.Log ($"[Settings Manager] UnknownSetting: {setting}");
                    return;
            }
            applyButton.interactable = true;
        }

        public float GetSetting (string setting) {
            //settings are written like shader directives
            switch (setting) {
                case "LIGHT_REALTIME":
                    return mainMaterial.GetFloat ("_EmissionFactor");

                case "LIGHT_INTENSITY":
                    return directionalLight.intensity;

                case "LIGHT_LATTITUDE":
                    return directionalLight.transform.rotation.eulerAngles.x;

                case "LIGHT_LONGITUDE":
                    return longitude.rotation.eulerAngles.y;

                case "SHADOW_ENABLE":
                    return UnityGraphicsBullshit.MainLightCastShadows ? 1 : 0;

                case "SHADOW_RESOLUTION":
                    if (UnityGraphicsBullshit.MainLightShadowResolution == UnityEngine.Rendering.Universal.ShadowResolution._256) return 0;
                    else if (UnityGraphicsBullshit.MainLightShadowResolution == UnityEngine.Rendering.Universal.ShadowResolution._512) return 1;
                    else if (UnityGraphicsBullshit.MainLightShadowResolution == UnityEngine.Rendering.Universal.ShadowResolution._1024) return 2;
                    else if (UnityGraphicsBullshit.MainLightShadowResolution == UnityEngine.Rendering.Universal.ShadowResolution._2048) return 3;
                    else if (UnityGraphicsBullshit.MainLightShadowResolution == UnityEngine.Rendering.Universal.ShadowResolution._4096) return 4;
                    else return -1;

                case "SHADOW_INTENSITY":
                    return directionalLight.shadowStrength;

                case "CAMERA_NEAR_SPEED":
                    return Core.CameraController.Current.MoveSpeedValues.x;

                case "CAMERA_FAR_SPEED":
                    return Core.CameraController.Current.MoveSpeedValues.y;

                case "FOG_DISTANCE":
                    return Mathf.InverseLerp (MIN_FOG_FAR_DISTANCE, MAX_FOG_FAR_DISTANCE, RenderSettings.fogEndDistance);

                case "FOG_ENABLE":
                    return RenderSettings.fog ? 1 : 0;


                //case "FX_SSAO":
                //    for (int x = 0 ; x < forwardRendererData.rendererFeatures.Count ; x++) {
                //        if (forwardRendererData.rendererFeatures[x].name == "SSAO") {
                //            return forwardRendererData.rendererFeatures[x].isActive ? 1 : 0;
                //        }
                //    }
                //    return 0;

                //case "FX_MOTIONBLUR":
                //    if (globalVolume.profile.TryGet (out MotionBlur motionBlur)) {
                //        return motionBlur.intensity.value;
                //    }
                //    return 0;

                default:
                    return 0;
            }
        }

        public void LoadSettings (float[] settings) {
            for (int i = 0 ; i < SETTINGS_NAMES.Length ; i++) {
                SetSetting (SETTINGS_NAMES[i], settings[i]);
            }
            //SerializeSettings ();
            presetWasLoaded?.Invoke ();
        }

        public void RevertSettings () {
            LoadSettings (DefaultSettings);
        }

        public void SerializeSettings () {
            IO.Serializer.SerializeSettings (GetSettings);
            applyButton.interactable = false;
        }

        public void Open () {
            if (visible) return;

            EscapeController.Current.AddEscapeAction (Close);
            transform.SetAsLastSibling ();
            if (enableUIBlocker) UIBlocker.Current.ActivateBlocker (gameObject);
            visible = true;
            gameObject.SetActive (true);
        }
        public virtual void Close () {
            if (!visible) return;

            if (enableUIBlocker) UIBlocker.Current.DeactivateBlocker ();
            visible = false;
            gameObject.SetActive (false);
        }

        public void Toggle () {
            if (visible) Close ();
            else Open ();
        }

        private void Awake () {
            longitude = directionalLight.transform.parent;
        }

        private void Start () {
            gameObject.SetActive (false);
        }
    }
}