using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Citybuilder.UI {
    public class MenuController : Singleton<MenuController> {
        [SerializeField] private TextMeshProUGUI versionLabel;
        [Space]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button newButton;
        [SerializeField] private Button quitButton;

        public void DisableLoadButton () {
            loadButton.interactable = false;
        }

        public void OpenWebsite () {
            Application.OpenURL (@"https://jorogames.itch.io/");
        }

        private void Awake () {

            //continueButton
            loadButton.onClick.AddListener (LoadWorldModal.Current.Open);
            newButton.onClick.AddListener (NewWorldModal.Current.Open);
            quitButton.onClick.AddListener (Core.GameManager.Current.Quit);
        }


        private void Start () {
            versionLabel.SetText (Application.version);
            continueButton.interactable = false;
        }
    }
}