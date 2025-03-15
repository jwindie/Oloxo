using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Citybuilder.UI {
    public class NewWorldModal : Singleton<NewWorldModal> {
        [SerializeField] private bool enableUIBlocker;
        [SerializeField] private GameObject customSizeSlider;
        private int mapSize;
        private Canvas canvas;

        private TextMeshProUGUI customSizeLabel;

        protected virtual void Awake () {
            customSizeLabel = customSizeSlider.GetComponentInChildren<TextMeshProUGUI> ();
            customSizeSlider.SetActive (false);

            customSizeSlider.GetComponentInChildren<Slider> ().onValueChanged.AddListener (SetCustomMapSize);
            customSizeLabel.SetText ("2x2");


            canvas = GetComponent<Canvas> ();
            canvas.enabled = false;
            SetMapSize (0);
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

        public void SetMapSize (int value) {
            if (value < 5 && customSizeSlider.activeInHierarchy) ShowCustomSlider (false);

            if (value == 0) {
                mapSize = 2;
            }
            else if (value == 1) {
                mapSize = 4;
            }
            else if (value == 2) {
                mapSize = 8;
            }
            else if (value == 3) {
                mapSize = 16;
            }
            else if (value == 4) {
                mapSize = 32;
            }
            else if (value == 5) {
                ShowCustomSlider (true);
            }
        }

        public void CreateWorld () {
            Close ();

            if (Core.GameManager.Current.IsMenuMode) {
                Core.GameManager.Current.LoadMap (mapSize);
            }
            else {
                Core.World.Current.CreateWorld (mapSize);
            }
        }

        public void ShowCustomSlider (bool state) {
            customSizeSlider.SetActive (state);
        }

        private void SetCustomMapSize (float value) {
            mapSize = (int) value;
            customSizeLabel.SetText ($"{mapSize}x{mapSize}");
        }
    }
}