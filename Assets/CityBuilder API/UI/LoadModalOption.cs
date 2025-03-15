using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Citybuilder.UI {
    public class LoadModalOption : MonoBehaviour {

        public struct LoadModalOptionData {
            public Texture2D icon;
            public string worldName;
            public Dictionary<string, string> metaData;
            public string path;
        }

        [SerializeField] private RawImage icon;
        [SerializeField] private TextMeshProUGUI worldName;
        [SerializeField] private TextMeshProUGUI worldInfo;

        private string path;

        private Button button;
        private Image bg;

        public string Path {
            get {
                return path;
            }
        }
        public void PopulateFields (string path, Texture2D texture, string name, Dictionary<string, string> metaData) {
            this.path = path;
            icon.texture = texture;
            worldName.SetText (name);

            //can only display FEW (3) lines of info
            string infoText = "";
            Debug.Log ($"Meta Length {metaData.Count}");
            if (metaData.ContainsKey ("saved")) {
                Debug.Log ("BREAK");
                infoText += $"Last Played: {metaData["saved"]}\n";
            }
            if (metaData.ContainsKey ("size")) {
                Debug.Log ("BREAK");
                infoText += $"Map Size: {metaData["size"]} x {metaData["size"]}\n";
            }

            worldInfo.SetText (infoText);

            button = GetComponent<Button> ();
            button.onClick.AddListener (SelectOption);

            bg = GetComponent<Image> ();
            Deselect ();
        }


        /// <summary>
        /// Selects the current option and tells the loadModalController
        /// </summary>
        private void SelectOption () {
            bg.color = Color.white;
            LoadWorldModal.Current.SelectOption (this);
        }

        public void Deselect () {
            bg.color = new Color (.8f, .8f, .8f, 1);
        }
    }
}