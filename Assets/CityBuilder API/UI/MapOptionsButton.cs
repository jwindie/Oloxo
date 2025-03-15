using Citybuilder.Core;
using Citybuilder.Testing;
using ModelShark;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Citybuilder.UI {
    public class MapOptionsButton : MonoBehaviour, IHideableUI {


        [Space]
        [SerializeField] private Button toggleButton;
        [SerializeField] private GameObject dropdown;
        [SerializeField] private bool disableAllOtherContextButtons;
        [SerializeField] private Color highlightColor;

        private bool dropdownVisible = false;

        void IHideableUI.Show (bool state) {
            ShowDropDown (state);
        }

        public void ShowDropDown (bool state) {
            if (state) { //show dropdown
                if (disableAllOtherContextButtons) HUDController.Current.ToggleAllHideableUI (false, this);
                dropdown.gameObject.SetActive (true);
                toggleButton.image.color = highlightColor;
                EscapeController.Current.AddEscapeAction (() => ShowDropDown (false));
            }
            else {
                toggleButton.image.color = Color.white;
                dropdown.gameObject.SetActive (false);
            }
            dropdownVisible = state;
        }

        public void LoadMap () {
            LoadWorldModal.Current.Open ();
        }

        public void SaveMap () {
            SaveModal.Current.Open ();
        }

        public void DeleteMap () {
            World.Current.ClearMap ();
        }

        public void CreateNewMap () {
            NewWorldModal.Current.Open ();
        }

        private void Awake () {
            HUDController.Current.AddHideableUI (this);
        }

        private void Start () {
            toggleButton.onClick.AddListener (() => ShowDropDown (!dropdownVisible));
            dropdown.transform.Find ("Load").GetComponent<Button> ().onClick.AddListener (LoadMap);
            dropdown.transform.Find ("Save").GetComponent<Button> ().onClick.AddListener (SaveMap);
            dropdown.transform.Find ("Delete").GetComponent<Button> ().onClick.AddListener (DeleteMap);
            dropdown.transform.Find ("Create").GetComponent<Button> ().onClick.AddListener (CreateNewMap);

            dropdown.SetActive (false);
        }
    }
}