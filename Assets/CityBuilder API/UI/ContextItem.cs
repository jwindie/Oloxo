using Citybuilder.Core;
using Citybuilder.Testing;
using ModelShark;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Citybuilder.UI {
    public class ContextItem : MonoBehaviour, IHideableUI {

        public static int ContextItemsOpen { get; protected set; } = 0;

        [SerializeField] protected Color highlightColor;
        [SerializeField] protected string setting;
        [SerializeField] protected bool allowAreaPainting;

        [Space]
        [SerializeField] protected Button toggleButton;
        [SerializeField] protected GameObject dropdown;
        [SerializeField] protected Button activeStateButton;
        [SerializeField] protected Button inactiveStateButton;
        [SerializeField] protected bool disableAllOtherContextButtons;
        [Space]
        [SerializeField] protected string tooltip = "";

        protected bool dropdownVisible = false;


        public void UpdateSetting (bool state) {
            MapEditor.Current.GetSetting (setting).state = state;
        }


        public virtual void Show (bool state) {
            ShowDropDown (state);
        }

        public void ShowDropDown (bool state) {
            if (dropdownVisible == state) return;

            if (state) { //show dropdown
                MapEditor.Current.GetSetting (setting).enabled = true;
                dropdown.gameObject.SetActive (true);
                toggleButton.image.color = highlightColor;
                Selector.Current.AreaDraggingEnabled = allowAreaPainting;
                EscapeController.Current.AddEscapeAction (() => ShowDropDown (false));
                ContextItemsOpen++;

                //close all others
                if (disableAllOtherContextButtons) HUDController.Current.ToggleAllHideableUI (false, this);
            }
            else {
                MapEditor.Current.GetSetting (setting).enabled = false;
                dropdown.gameObject.SetActive (false);
                toggleButton.image.color = Color.white;
                ContextItemsOpen--;
            }
            dropdownVisible = state;

            //if there are no context options open, reset the reticule
            if (ContextItemsOpen == 0) Selector.Current.ResetArea ();
        }

        protected void Awake () {
            HUDController.Current.AddHideableUI (this);
        }

        protected virtual void Start () {
            toggleButton.onClick.AddListener (() => ShowDropDown (!dropdownVisible));
            activeStateButton.onClick.AddListener (() => {
                UpdateSetting (true);
                //set color and deactivate the other button
                activeStateButton.image.color = highlightColor;
                inactiveStateButton.image.color = Color.white;
            });
            inactiveStateButton.onClick.AddListener (() => {
                UpdateSetting (false);
                //set color and deactivate the other button
                inactiveStateButton.image.color = highlightColor;
                activeStateButton.image.color = Color.white;
            });

            dropdown.SetActive (false);
            inactiveStateButton.image.color = highlightColor;
        }

        protected virtual void Update () {
            if (dropdownVisible) {
                if (Input.GetKeyDown (InputMap.TOGGLE_PAINT_STATE)) {
                    if (MapEditor.Current.GetSetting (setting).state ) { //state is on
                        inactiveStateButton.onClick.Invoke ();
                    }
                    else {
                        activeStateButton.onClick.Invoke ();
                    }
                }
            }
        }
    }
}