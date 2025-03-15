using Citybuilder.Core;
using System.IO;
using UnityEngine;

namespace Citybuilder.IO {
    public static class Serializer {

        public const int WORLD_SAVE_FORMAT = 1;

        static Serializer () {
            //enusre the folders exist in the dataPath
            Directory.CreateDirectory (Paths.DATA_PATH);
            Directory.CreateDirectory (Paths.WORLD_SAVE_PATH);
            Directory.CreateDirectory (Paths.MENU_SAVE_PATH);
        }

        public static void SerializeSettings (float[] settings) {
            //write all in binary
            using (BinaryWriter writer = new BinaryWriter (File.Open (Paths.DATA_PATH + "/settings.data", FileMode.Create))) {
                for (int i = 0 ; i < settings.Length ; i++) {
                    writer.Write (settings[i]);
                }
            }
        }

        public static float[] DeserializeSettings () {

            //check to see if the settings exist
            //if not just load the default settings
            GameManager.Current.FileLog.Write ("Deserializing settings file...");
            if (File.Exists (Paths.DATA_PATH + "/settings.data")) {

                try {
                    float[] settings = new float[SettingsManager.SETTINGS_NAMES.Length];
                    using (BinaryReader reader = new BinaryReader (File.Open (Paths.DATA_PATH + "/settings.data", FileMode.Open))) {

                        for (int i = 0 ; i < SettingsManager.SETTINGS_NAMES.Length ; i++) {
                            if (reader.BaseStream.Position != reader.BaseStream.Length - 1) {
                                settings[i] = reader.ReadSingle ();
                            }
                        }
                    }
                    if (settings.Length == SettingsManager.SETTINGS_NAMES.Length) {
                        GameManager.Current.FileLog.WriteLine ("Success!", false);
                        return settings;
                    }
                }
                catch {
                }
            }
            GameManager.Current.FileLog.WriteLine ("Failed! Loading default preset.", false);
            return SettingsManager.DefaultSettings;
        }

        public static void SerializeMap (string mapName, Texture2D thumbnail) {

            //create save folder
            string worldFolderPath = $"{Paths.WORLD_SAVE_PATH}/{mapName}";
            Directory.CreateDirectory (worldFolderPath);

            using (StreamWriter writer = new StreamWriter ($"{worldFolderPath}/info.txt")) {
                writer.WriteLine ($"name = {mapName}");
                foreach (string s in World.Current.GetWorldSpecificMetaData ()) {
                    writer.WriteLine (s);
                }
                writer.WriteLine ($"saved = {System.DateTime.Now}");
            }

            //write all in binary
            using (BinaryWriter writer = new BinaryWriter (File.Open ($"{worldFolderPath}/world.world", FileMode.Create))) {

                writer.Write (WORLD_SAVE_FORMAT);
                World.Current.SerializeAllTiles (writer);
            }

            //try and get a snapshot of the world
            if (thumbnail) {
                byte[] bytes = thumbnail.EncodeToPNG ();
                File.WriteAllBytes ($"{worldFolderPath}/thumbnail.png", bytes);
            }

            //debug open the file we just wrote
            System.Diagnostics.Process.Start (worldFolderPath);

        }
        /// <summary>
        /// Deserializes a map form an explicit path incliding the extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DeserializeMapExplicit (string path) {

            GameManager.Current.FileLog.Write ($"Deserializing '{path}'...");

            if (File.Exists ($"{path}")) {

                using (BinaryReader reader = new BinaryReader (File.Open ($"{path}", FileMode.Open))) {
                    World.Current.DeserializeWorld (reader, reader.ReadInt32 ());
                    GameManager.Current.FileLog.WriteLine ($"Success!", false);
                    return true;
                }
            }
            else {
                //log out that there was a problem creating the world
                GameManager.Current.FileLog.WriteLine ($"Failed! File was missing.", false);
                return false;
            }
        }
        public static bool DeserializeMap (string mapName) {
            return DeserializeMapExplicit ($"{Paths.WORLD_SAVE_PATH}/{mapName}.world");
        }

        public static string[] GetMenuSaves () {
            return Directory.GetFiles (Paths.MENU_SAVE_PATH, "*.world");
        }
    }
}