using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Citybuilder.UI {
    public class LoadWorldModal : Singleton<LoadWorldModal> {

                [SerializeField] private LoadModalOption template;
        [SerializeField] private Button loadWorldButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private bool enableUIBlocker;

        private Canvas canvas;
        private LoadModalOption selectedOption = null;
        private List<LoadModalOption> options = new List<LoadModalOption> ();
        private Stack<LoadModalOption> optionsPool = new Stack<LoadModalOption> ();

        public void SelectOption (LoadModalOption option) {

            if (selectedOption == option) {
                //deselect all options
                DeselectAll ();
            }
            else {
                selectedOption = option;

                //deselect all other options
                foreach (LoadModalOption other in options) {
                    if (other != option) other.Deselect ();
                }
                loadWorldButton.interactable = true;
                deleteButton.interactable = true;
            }
        }

        public void DeselectAll () {
            SelectOption (null);
            loadWorldButton.interactable = false;
            deleteButton.interactable = false;
        }

        public void LoadSelectedWorld () {
            if (selectedOption != null) {
                Close ();
                if (Core.GameManager.Current.IsMenuMode) {
                    Core.GameManager.Current.LoadMap ($"{selectedOption.Path}/world.world");
                }
                else {
                    IO.Serializer.DeserializeMapExplicit ($"{selectedOption.Path}/world.world");
                }
            }
        }
        public void TryDeleteSelectedWorld () {
            if (selectedOption != null) {

                //promt a modal popup for this
                GenericModal.Current.SetHeader ("Delete World");
                GenericModal.Current.SetBody ("This will <b>permanently</b> remove the world from your computer. " +
                    "This cannot be undone.\nAre you sure?");
                GenericModal.Current.SetContextButtons (
                    new GenericModal.ModalContextButtonData {
                        defaultColor = Color.white,
                        label = "Cancel",
                        onClick = GenericModal.Current.Close
                    },
                    new GenericModal.ModalContextButtonData {
                        defaultColor = GenericModal.ModalRed,
                        label = "Delete",
                        onClick = () => DeleteSelectedWorld (selectedOption.Path)
                    }
                    );
                GenericModal.Current.Open ();
            }
        }
        public void GenerateOptions () {

            //if there are already options, pool them the regeretate
            if (options.Count > 0) {
                foreach (LoadModalOption option in options) {
                    option.gameObject.SetActive (false);
                    optionsPool.Push (option);
                }
                options.Clear ();
            }

            //use the IO.FileHandler to locate the files
            var dataArray = IO.FileHandler.GetLoadOptions ();

            foreach (LoadModalOption.LoadModalOptionData data in dataArray) {

                #region Debug Logging (Disabled)
                //Core.GameManager.Current.FileLog.WriteLine (data.worldName);
                //Core.GameManager.Current.FileLog.WriteLine (data.icon == null ? "Texture: NULL" : $"Texture: {data.icon.width}x{data.icon.height}");

                //foreach (string line in data.worldInfo) {
                //    Core.GameManager.Current.FileLog.WriteLine (line);
                //}
                #endregion

                //createa a LoadModalOption
                LoadModalOption newOption = Instantiate (template.gameObject, template.transform.parent).GetComponent<LoadModalOption> ();
                options.Add (newOption);
                newOption.gameObject.SetActive (true);
                newOption.PopulateFields (data.path, data.icon, data.worldName, data.metaData);
            }

            loadWorldButton.interactable = false;
            deleteButton.interactable = false;
        }

        public void Open () {
            if (canvas.enabled == true) return;

            //refresh the maps
            GenerateOptions ();

            EscapeController.Current.AddEscapeAction (Close);
            transform.SetAsLastSibling ();
            if (enableUIBlocker) UIBlocker.Current.ActivateBlocker (gameObject);
            canvas.enabled = true;
            Core.CameraController.Current.LockCamera ();
        }
        public void Close () {
            if (canvas.enabled == false) return;

            if (enableUIBlocker) UIBlocker.Current.DeactivateBlocker ();
            canvas.enabled = false;
            Core.CameraController.Current.LockCamera (false);
        }

        private void Start () {
            template.gameObject.SetActive (false);
            GenerateOptions ();

            //if there are no options, hide the load button
            if (options.Count == 0) MenuController.Current.DisableLoadButton ();
        }

        private void Awake () {
            loadWorldButton.onClick.AddListener (LoadSelectedWorld);
            refreshButton.onClick.AddListener (GenerateOptions);
            deleteButton.onClick.AddListener (TryDeleteSelectedWorld);

            canvas = GetComponent<Canvas> ();
            canvas.enabled = false;
        }

        private void DeleteSelectedWorld (string path) {

            //note: A modal window SHOULD already be open

            //try to delete the world through file handler
            int result = IO.FileHandler.DeleteDirectory (path);

            if (result == 0) { //failed to delete
                GenericModal.Current.SetHeader ("Error");
                GenericModal.Current.SetBody ("The world could not be deleted. :/");
                GenericModal.Current.SetContextButtons (
                    new GenericModal.ModalContextButtonData {
                        defaultColor = Color.white,
                        label = "OK",
                        onClick = GenericModal.Current.Close
                    }
                    );
            }
            else if (result == 1) {
                //close the modal window
                GenericModal.Current.Close ();

                //hide option and deselect
                options.Remove (selectedOption);
                selectedOption.gameObject.SetActive (false);
                optionsPool.Push (selectedOption);
                DeselectAll ();

                //check to see if there are any saves left
                //if not, hide the load option from the main menu
                if (options.Count == 0) {
                    MenuController.Current.DisableLoadButton ();
                }
            }
            else if (result == -1) { //wold was not found
                GenericModal.Current.SetHeader ("Error");
                GenericModal.Current.SetBody ("The world could not be found :/");
                GenericModal.Current.SetContextButtons (
                    new GenericModal.ModalContextButtonData {
                        defaultColor = Color.white,
                        label = "OK",
                        onClick = GenericModal.Current.Close
                    }
                    );
            }
        }
    }
}