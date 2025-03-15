using UnityEngine;
using UnityEngine.UI;
using Citybuilder.Core;

namespace Citybuilder.Testing {
    public class UndoButton : MonoBehaviour {

        [SerializeField] private Button button;
        [SerializeField] private Text undoDepthLabel;
        [SerializeField] private bool undoEvents;
        [SerializeField] private bool redoEvents;

        private void Awake () {
            if (undoEvents) {
                button.onClick.AddListener (() => UndoHandler.ExecuteUndo ());
                UndoHandler.UpdateUndoStackDepth += OnUpdateUndoDepth;
                OnUpdateUndoDepth (0);
            }
            if (redoEvents) {
                button.onClick.AddListener (() => UndoHandler.ExecuteUndo ());
                UndoHandler.UpdateUndoStackDepth += OnUpdateRedoDepth;
                OnUpdateRedoDepth (0);
            }
        }

        private void OnUpdateUndoDepth (int depth) {
            undoDepthLabel.text = depth.ToString ();
            undoDepthLabel.transform.parent.gameObject.SetActive (depth > 1);
            button.interactable = depth > 0;
        }

        private void OnUpdateRedoDepth (int depth) {
            undoDepthLabel.text = depth.ToString ();
            undoDepthLabel.transform.parent.gameObject.SetActive (depth > 1);
            button.interactable = depth > 0;
        }
    }
}