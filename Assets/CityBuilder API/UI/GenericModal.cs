using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Citybuilder.UI {
    public class GenericModal : Singleton<GenericModal> {

        public static readonly Color ModalRed = new Color (.858f, .684f, .684f, 1);
        public static readonly Color ModalGreen = new Color (.684f, .858f, .684f, 1);

        public struct ModalContextButtonData {
            public Color defaultColor;
            public string label;
            public System.Action onClick;
        }


        [SerializeField] private bool enableUIBlocker;
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI body;

        [SerializeField] private Button[] contextButtons;


        private Canvas canvas;

        protected virtual void Awake () {
            canvas = GetComponent<Canvas> ();
            canvas.enabled = false;
        }

        public void Open () {
            if (canvas.enabled == true) return;

            EscapeController.Current.AddEscapeAction (Close);
            transform.SetAsLastSibling ();
            if (enableUIBlocker) UIBlocker.Current.ActivateBlocker (gameObject);
            canvas.enabled = true;
            Core.CameraController.Current.LockCamera ();
        }
        public virtual void Close () {
            if (canvas.enabled == false) return;

            if (enableUIBlocker) UIBlocker.Current.DeactivateBlocker ();
            canvas.enabled = false;
            Core.CameraController.Current.LockCamera (false);
        }


        public void SetHeader (string text) {
            header.SetText (text);
        }
        public void SetBody (string text) {
            body
                .SetText (text);
        }

        public void SetContextButtons (params ModalContextButtonData[] buttonData) {
            //disable all buttons
            foreach (Button b in contextButtons) b.gameObject.SetActive (false);

            for (int i = 0 ; i < buttonData.Length ; i++) {
                if (i == contextButtons.Length) break;
                SetContextButton (i, buttonData[i]);
            }
        }

        private void SetContextButton (int buttonIndex, ModalContextButtonData data) {
            contextButtons[buttonIndex].gameObject.SetActive (true);

            contextButtons[buttonIndex].onClick.RemoveAllListeners ();
            contextButtons[buttonIndex].onClick.AddListener (() => data.onClick ());

            contextButtons[buttonIndex].image.color = data.defaultColor;
            contextButtons[buttonIndex].GetComponentInChildren<TextMeshProUGUI> ().SetText (data.label);
        }
    }
}
