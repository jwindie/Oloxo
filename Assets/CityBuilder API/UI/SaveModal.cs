using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Citybuilder.UI {
    public class SaveModal : Singleton<SaveModal> {

        public struct ModalContextButtonData {
            public Color defaultColor;
            public string label;
            public System.Action onClick;
        }


        [SerializeField] private bool enableUIBlocker;
        [SerializeField] private TextMeshProUGUI warningMessage;
        [SerializeField] private TMP_InputField inputField;

        [SerializeField] private Button saveButton;

        private TextMeshProUGUI saveButtonLabel;
        private Canvas canvas;

        protected virtual void Awake () {
            canvas = GetComponent<Canvas> ();
            canvas.enabled = false;
            warningMessage.SetText ("");

            saveButtonLabel = saveButton.GetComponentInChildren<TextMeshProUGUI> ();
            saveButtonLabel.SetText ("Save");
            saveButton.interactable = false;
            inputField.onValueChanged.AddListener (OnEndEdit);
        }

        public void Open () {
            if (canvas.enabled == true) return;

            EscapeController.Current.AddEscapeAction (Close);
            transform.SetAsLastSibling ();
            if (enableUIBlocker) UIBlocker.Current.ActivateBlocker (gameObject);
            canvas.enabled = true;
            Core.CameraController.Current.LockCamera ();
            inputField.ActivateInputField ();
            OnEndEdit (inputField.text);

        }
        public virtual void Close () {
            if (canvas.enabled == false) return;

            if (enableUIBlocker) UIBlocker.Current.DeactivateBlocker ();
            canvas.enabled = false;
            Core.CameraController.Current.LockCamera (false);
            if (inputField.IsActive ()) inputField.DeactivateInputField ();
        }

        public void OnEndEdit (string path) {
            //verify that this path exists
            var result = IO.FileHandler.VerifyPathForSave (path);
            if (result == IO.FileHandler.PathForSaveVerificationResult.Invalid) {
                warningMessage.SetText ("Invalid save name");
                saveButton.interactable = false;
                saveButtonLabel.SetText ("Save");
            }
            else if (result == IO.FileHandler.PathForSaveVerificationResult.Exist) {
                warningMessage.SetText ("Save already exists. Overwrite?");
                saveButton.interactable = true;
                saveButtonLabel.SetText ("Overwrite");
            }
            else if (result == IO.FileHandler.PathForSaveVerificationResult.NotExist) {
                warningMessage.SetText ("");
                saveButton.interactable = true;
                saveButtonLabel.SetText ("Save");
            }
        }

        public void SaveMap () {
            Close ();
            Core.GameManager.Current.HideGUI (true);
            Core.Selector.Current.Hide ();
            StartCoroutine (WaitToSave ());
        }


        private IEnumerator WaitToSave () {
            float wait = 1;
            float fov = Core.CameraController.Current.RenderCamera.fieldOfView;
            Core.CameraController.Current.RenderCamera.fieldOfView = 40;
            while (wait > 0) {
                wait--;
                yield return null;
            }
            var texRaw = ScreenCapture.CaptureScreenshotAsTexture ();
            var croppedTex = B83.TextureTools.TextureTools.ResampleAndCrop (texRaw, texRaw.height, texRaw.height);

            Core.CameraController.Current.RenderCamera.fieldOfView = fov;
            IO.Serializer.SerializeMap (inputField.text, croppedTex);
            Core.GameManager.Current.HideGUI (false);
            Core.Selector.Current.Hide (false);
        }
    }
}
