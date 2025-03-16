using UnityEngine;

namespace Oloxo.Data {
    /// <summary>
    /// Helper class for converting data and references to commonly used paths.
    /// </summary>
    public static class DataHandler {

        public const string GAME_DATA_DIRECTORY = "gameData";
        public const string LOGS = GAME_DATA_DIRECTORY + "/logs";

        public const string SESSION_FILENAME = "session.kda";

        public static readonly string SESSION_PATH = $"{GAME_DATA_DIRECTORY}/{SESSION_FILENAME}";


        public static bool DataPathIsPersistent { get; private set; }
        public static string DataPath {
            get {
#if UNITY_EDITOR
                return DataPathIsPersistent ? Application.persistentDataPath : Application.dataPath;
#endif
                return Application.persistentDataPath;
            }
        }

        public static void UsePersistentDatapath (bool state) {
            DataPathIsPersistent = state;
        }
    }
}