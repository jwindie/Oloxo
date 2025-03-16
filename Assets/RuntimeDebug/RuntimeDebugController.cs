//Author: Jordan Williams
//Date: Sep 16 2020

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Joro.RuntimeDebugger {

    [RequireComponent (typeof (Canvas))]
    public class RuntimeDebugController : MonoBehaviour {
        public KeyCode toggleKey;
        public Font font;
        bool showConsole = false;
        bool showHelp = false;
        Canvas canvas;
        InputField inputField;
        GameObject helpWindow;

        #region Commands
        RuntimeDebugCommand help;
        RuntimeDebugCommand quit;
        static Action<string> OnSubmitCommandRaw;
        #endregion

        static List<object> commandList = new List<object> ();

        public static void AddCommand (object command) {
            commandList.Add (command);
        }

        private void Awake () {
            OnSubmitCommandRaw = (string command) => { HandleInput (command); };
            canvas = GetComponent<Canvas> ();
            canvas.enabled = showConsole;

            inputField = GetComponentInChildren<InputField> ();
            inputField.onEndEdit.AddListener ((string input) => HandleInput (input));

            helpWindow = GetComponentInChildren<ScrollRect> ().gameObject;
            helpWindow.SetActive (false);

            #region Commands
            help = new RuntimeDebugCommand ("help", "lists all commands available", "help", () => {
                ToggleHelpDialogue ();
            });
            quit = new RuntimeDebugCommand ("quit", "quits the application", "quit", () => {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit (0);
            });
            #endregion

            commandList = new List<object> {
                help,
                quit
            };

            //ensure that no two commands have the same ID
            List<string> commandIDs = new List<string> (commandList.Count);

            foreach (object o in commandList) {
                RuntimeDebugCommandBase commandBase = (o as RuntimeDebugCommandBase);
                if (commandIDs.Contains (commandBase.CommandId)) {
                    //two commadns have the same id, throw error
                    Debug.LogWarning ("There are commands with the same CommandID in the command list. This may cause unexpected behavior.");
                    break; //no need to continue, user was alerted
                }
                else commandIDs.Add (commandBase.CommandId);    //add the id to the list and continue with check
            }
        }

        private void Start () {
            //sort the commands
            commandList.Sort ((a, b) => (a as RuntimeDebugCommandBase).CommandFormat.CompareTo ((b as RuntimeDebugCommandBase).CommandFormat));
        }

        private void Update () {
            if (Input.GetKeyDown (toggleKey)) {

                if (showConsole) {
                    inputField.textComponent.text = ""; //clear text on open
                    inputField.ActivateInputField ();
                }
                else {
                    inputField.DeactivateInputField ();
                }

                showConsole = !showConsole;
                canvas.enabled = showConsole;
            }
        }

        public static void InputCommandDirect (string command) {
            if (OnSubmitCommandRaw != null) {
                OnSubmitCommandRaw.Invoke (command);
            }
        }

        private void HandleInput (string input) {
            if (input == null || input.Length < 1) return;

            string[] properties = input.Split (' ');

            for (int i = 0 ; i < commandList.Count ; i++) {

                //get debug command base object 
                RuntimeDebugCommandBase commandBase = commandList[i] as RuntimeDebugCommandBase;

                //does the input string contain the commadn id?
                if (properties[0] == (commandBase.CommandId)) {

                    //this needs to NOT break the game
                    //try {
                    //cast the command into a normal comamnd and invoke it
                    if (commandList[i] as RuntimeDebugCommand != null) {
                        (commandList[i] as RuntimeDebugCommand).Invoke ();
                    }
                    else if (commandList[i] as RuntimeDebugCommand<int> != null) {
                        (commandList[i] as RuntimeDebugCommand<int>).Invoke (int.Parse (properties[1]));
                    }
                    else if (commandList[i] as RuntimeDebugCommand<int, int> != null) {
                        (commandList[i] as RuntimeDebugCommand<int, int>).Invoke (int.Parse (properties[1]), int.Parse (properties[2]));
                    }
                    else if (commandList[i] as RuntimeDebugCommand<int, float> != null) {
                        (commandList[i] as RuntimeDebugCommand<int, float>).Invoke (int.Parse (properties[1]), float.Parse (properties[2]));
                    }
                    else if (commandList[i] as RuntimeDebugCommand<int, int, int> != null) {
                        (commandList[i] as RuntimeDebugCommand<int, int, int>).Invoke (int.Parse (properties[1]), int.Parse (properties[2]), int.Parse (properties[3]));
                    }
                    else if (commandList[i] as RuntimeDebugCommand<float> != null) {
                        (commandList[i] as RuntimeDebugCommand<float>).Invoke (float.Parse (properties[1]));
                    }
                    else if (commandList[i] as RuntimeDebugCommand<bool> != null) {
                        (commandList[i] as RuntimeDebugCommand<bool>).Invoke (bool.Parse (properties[1]));
                    }
                    else if (commandList[i] as RuntimeDebugCommand<int, int, float> != null) {
                        (commandList[i] as RuntimeDebugCommand<int, int, float>).Invoke (int.Parse (properties[1]), int.Parse (properties[2]), float.Parse (properties[3]));
                    }
                    else if (commandList[i] as RuntimeDebugCommand<string> != null) {
                        (commandList[i] as RuntimeDebugCommand<string>).Invoke (properties[1]);
                    }
                    else if (commandList[i] as RuntimeDebugCommand<string, float> != null) {
                        (commandList[i] as RuntimeDebugCommand<string, float>).Invoke (properties[1], float.Parse (properties[2]));
                    }
                    else if (commandList[i] as RuntimeDebugCommand<string, string, float> != null) {
                        (commandList[i] as RuntimeDebugCommand<string, string, float>).Invoke (properties[1], properties[2], float.Parse (properties[3]));
                    }
                    else {
                        //could not run command
                        Debug.LogError ($"Could not process command '{commandBase.CommandId}' of type: {commandBase.GetType ()} (Arguments: {properties.Length - 1})! Not supported?");
                    }
                    //}
                    //catch {
                    //    Debug.Log ($"Command execution failed: {input.Replace (' ', '_')}");
                    //}
                }
            }

            inputField.SetTextWithoutNotify ("");
            inputField.ActivateInputField ();
        }

        private void ToggleHelpDialogue () {
            showHelp = !showHelp;
            helpWindow.SetActive (showHelp);
            if (showHelp) {
                System.Text.StringBuilder builder = new System.Text.StringBuilder ();
                RuntimeDebugCommandBase commandBase = null;
                for (int i = 0 ; i < commandList.Count ; i++) {
                    commandBase = (commandList[i] as RuntimeDebugCommandBase);
                    builder.AppendLine ($"{commandBase.CommandFormat} <color=#ff9900><i>{commandBase.CommandDescription}</i></color>");
                    helpWindow.GetComponentInChildren<Text> ().text = builder.ToString ();
                }
            }
            else {
                helpWindow.GetComponentInChildren<Text> ().text = "";
            }
        }

        private void OnValidate () {
            //get all text objects and set the font
            foreach (Text t in GetComponentsInChildren<Text>()) {
                t.font = font;
            }
        }
    }
}
