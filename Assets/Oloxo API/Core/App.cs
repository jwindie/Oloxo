using Joro.RuntimeDebugger;
using Oloxo.Data;
using Oloxo.Global;
using System.IO;
using UnityEngine;

namespace Oloxo.Core {

    /// <summary>
    /// Main entry point to the game. Nothing in the polorolo namespace kicks off 
    /// if there isnt an app class somewhere.
    /// </summary>
    public class App : SingletonComponent<App> {

        private enum DatapathLocation { Persistent, Nonpersistent };

        //INSPECTOR
        [SerializeField] private DatapathLocation dataPathLocation;
        [SerializeField] private bool openDataPathOnAwake;
        [Tooltip("Should the logger show processes happening withing the serializer?")]
        [SerializeField] private bool logSerialProcesses;

        public AudioHandler AudioHandler { get; private set; }
        public InputHandler InputHandler { get; private set; }
        public LogHandler LogHandler { get; private set; }
        public GameManager Game { get; private set; }

        /// <summary>
        /// Main awake. This is the first to run and the actual entry program for the game.
        /// </summary>
        void Awake () {

            //prevents the unity debug scene from loading
            UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;

            SetSingletonInstance (this);

            DataHandler.UsePersistentDatapath (dataPathLocation == DatapathLocation.Persistent);
            ValidateOrCreateRootDirectory ();

            LogHandler = new LogHandler ($"{DataHandler.DataPath}/{DataHandler.LOGS}");
            LogHandler.Log ($"[INIT] Started app ({Application.productName} version {Application.version})");
            LogHandler.Space ();
            LogSystemInfo ();

            Serializer.SetPath ($"{DataHandler.DataPath}/{ DataHandler.GAME_DATA_DIRECTORY}");
            Serializer.logSerialProcesses = logSerialProcesses;

            if (openDataPathOnAwake) System.Diagnostics.Process.Start ($"{DataHandler.DataPath}/{DataHandler.GAME_DATA_DIRECTORY}");

            //add custom commands for the runtime debugger
            CustomRuntimeDebugCommands.LoadCommands ();
            LogHandler.Log ($"[LOAD] Loaded custom Runtime Debugger Commands");

            AudioHandler = new AudioHandler ();
            InputHandler = new InputHandler ();
            Game = new GameManager ();

            Game.Initialize ();
        }

        private void OnDestroy () {
            LogHandler.Close ();
        }

        /// <summary>
        /// Returns true if game files were found. If not, it creates the necessary files
        /// </summary>
        /// <returns></returns>
        private bool ValidateOrCreateRootDirectory () {
            if (Directory.Exists ($"{DataHandler.DataPath}/{DataHandler.GAME_DATA_DIRECTORY}")) return true;

            Directory.CreateDirectory ($"{DataHandler.DataPath}/{DataHandler.GAME_DATA_DIRECTORY}");
            return false;
        }

        private void LogSystemInfo () {
            string indent = $"{LogHandler.GetTimestamp ()} [SYSTEM] ";

            // Operating System Info
            LogHandler.StartBlock ("[SYSTEM] System Info:");
            //devce
            LogHandler.Log ($"[SYSTEM] {SystemInfo.deviceModel} ({SystemInfo.deviceType})");
            //operating systm
            LogHandler.Log ($"[SYSTEM] Operating System: {SystemInfo.operatingSystem}");
            LogHandler.Log ($"[SYSTEM] Processor: {SystemInfo.processorType} ({SystemInfo.processorCount} Cores)");
            LogHandler.Log ($"[SYSTEM] System Memory (MB): {SystemInfo.systemMemorySize}");
            // Graphics Card Info (with API type in the format requested)
            LogHandler.Log ($"[SYSTEM] Graphics Device: {SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsDeviceType})");
            LogHandler.Log ($"[SYSTEM] Graphics Memory Size (MB): {SystemInfo.graphicsMemorySize}");
            LogHandler.EndBlock ();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Called simply to ensure this si the first ibject in the hierachy and properly named.
        /// </summary>
        private void OnValidate () {
            name = "App";
            transform.SetSiblingIndex (0);
        }
#endif
    }
}