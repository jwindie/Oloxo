using System;
using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Core {
    public static class UndoHandler {

        public const int MAX_UNDO_HISTORY_SIZE = 64;
        public static Action<int> UpdateUndoStackDepth;

        private static LinkedStack<List<Action>> undoStack = new LinkedStack<List<Action>> ();
        private static LinkedList<string> undoStackNames = new LinkedList<string> ();
        private static string undoName;
        private static List<Action> pendingUndoCommands;

        public static int Count {
            get {
                return undoStack.Count;
            }
        }


        public static void StartUndo (string name) {
            pendingUndoCommands = new List<Action> ();
            //Debug.Log ("Started Undo!");

        }

        public static void RegisterUndo (Action action) {
            if (pendingUndoCommands != null) {
                pendingUndoCommands.Add (action);
                //Debug.Log ("Added Undo!");
            }
        }

        public static void SubmitUndo () {
            if (pendingUndoCommands != null && pendingUndoCommands.Count > 0) {

                //if there are too many undos, remove the oldest entry
                if (undoStack.Count == MAX_UNDO_HISTORY_SIZE) {
                    undoStack.RemoveFirst ();
                }

                //Debug.Log ($"Submitted Undo! {undoCommands.Count}");
                undoStack.Push (new List<Action> (pendingUndoCommands));
                undoStackNames.AddLast (undoName);

                UpdateUndoStackDepth?.Invoke (undoStack.Count);
            }

            pendingUndoCommands = null;
            undoName = "Null";
        }

        public static void ExecuteUndo () {
            if (undoStack.Count > 0) {
                undoStackNames.RemoveLast ();
                foreach (Action action in undoStack.Pop ()) {
                    action?.Invoke ();
                }
                Debug.Log ("Reverted Undo!");
                UpdateUndoStackDepth?.Invoke (undoStack.Count);
            }
        }

        public static void GetNextUndoMoveInfo (out string name, out int actions) {
            if (undoStack.Count > 0) {
                name = undoStackNames.Last.Value;
                actions = undoStack.Peek ().Count;
            }
            else {
                name = "Null";
                actions = 0;
            }
        }

        public static void ClearUndoHistory () {
            undoStack.Clear ();
            undoStackNames.Clear ();
            UpdateUndoStackDepth?.Invoke (undoStack.Count);
        }
    }
}
