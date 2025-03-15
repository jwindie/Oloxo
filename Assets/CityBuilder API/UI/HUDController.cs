using UnityEngine;
using Citybuilder.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Animations;

namespace Citybuilder.UI {
    public class HUDController : Singleton<HUDController> {


        private const float VISIBLE_Y_POSITION = 50;
        private const float HIDDEN_Y_POSITION = -150;
        private const float ANIMATION_EASE = .2f;

        [SerializeField] private bool recordUndos = false;
        //[SerializeField] private GameObject settingsWindow;

        private RectTransform rectTransform;

        //private bool settingsVisibility;
        private bool hidden = false;

        private float velocity;

        private List<IHideableUI> hideableUIs = new List<IHideableUI> ();

        //public void ToggleSettings () {
        //    if (settingsVisibility) {
        //        settingsVisibility = false;
        //        settingsWindow.SetActive (false);
        //        UIBlocker.Current.DeactivateBlocker ();
        //    }
        //    else {
        //        settingsVisibility = true;
        //        settingsWindow.SetActive (true);
        //        UIBlocker.Current.ActivateBlocker (gameObject);
        //        EscapeController.Current.AddEscapeAction (() => ToggleSettings (false));
        //    }
        //}

        //public void ToggleSettings (bool state) {
        //    settingsVisibility = !state;
        //    ToggleSettings ();
        //}

        public void Hide (bool state = true) {
            if (state != hidden) {
                StopAllCoroutines ();
                hidden = state;
                StartCoroutine (HideGUI (state));

                //close any open paint submenus
                if (state) {
                    ToggleAllHideableUI (false, null);
                }
            }
        }

        public void AddHideableUI (IHideableUI item) {
            hideableUIs.Add (item);
        }

        public void ToggleAllHideableUI (bool state, IHideableUI exclude) {

            foreach (IHideableUI item in hideableUIs) if (item != exclude) item.Show (state);

            //if there are no context options open, reset the reticule
            if (ContextItem.ContextItemsOpen == 0) Selector.Current.ResetArea ();
        }

        private void Awake () {
            rectTransform = GetComponent<RectTransform> ();
        }

        private void Update () {
            if (recordUndos) {
                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()) {
                    if (Input.GetMouseButtonDown (0)) UndoHandler.StartUndo ("Edit World");
                }
                if (Input.GetMouseButtonUp (0)) UndoHandler.SubmitUndo ();
            }

            //handle escaping
            if (Input.GetKeyDown (KeyCode.Escape)) {
                EscapeController.Current.HandleEscape ();
            }

            //Undo Hotkey
            if (UndoHandler.Count > 0) {
                if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {
                    if (Input.GetKeyDown (KeyCode.Z)) {
                        UndoHandler.ExecuteUndo ();
                    }
                }
            }
        }
        private IEnumerator HideGUI (bool state) {
            if (state) {
                while (rectTransform.anchoredPosition.y != HIDDEN_Y_POSITION) {
                    var _ = rectTransform.anchoredPosition;
                    _.y = Mathf.SmoothDamp (_.y, HIDDEN_Y_POSITION, ref velocity, ANIMATION_EASE);

                    if (Mathf.Abs (_.y - HIDDEN_Y_POSITION) < .00f) {
                        _.y = HIDDEN_Y_POSITION;
                    }
                    rectTransform.anchoredPosition = _;
                    yield return new WaitForEndOfFrame ();
                }
            }
            else {
                while (rectTransform.anchoredPosition.y != VISIBLE_Y_POSITION) {
                    var _ = rectTransform.anchoredPosition;
                    _.y = Mathf.SmoothDamp (_.y, VISIBLE_Y_POSITION, ref velocity, ANIMATION_EASE);

                    if (Mathf.Abs (_.y - VISIBLE_Y_POSITION) < .00f) {
                        _.y = VISIBLE_Y_POSITION;
                    }
                    rectTransform.anchoredPosition = _;
                    yield return new WaitForEndOfFrame ();
                }
            }
        }
    }
}