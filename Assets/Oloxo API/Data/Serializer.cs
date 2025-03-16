using Oloxo.Core;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Oloxo.Data {

    /// <summary>
    /// Handles serialization of .kda files and <see cref="KeyedDataBlock"/>s.    /// </summary>
    public static class Serializer {

        private static readonly Regex KeyValueRegex = new Regex (@"^\s*(\w+)\s*[:]\s*(.*)\s*$", RegexOptions.Compiled);
        private static readonly Regex ListStartRegex = new Regex (@"^\s*(\w+)\s*{", RegexOptions.Compiled);
        private static readonly Regex ListEndRegex = new Regex (@"^\s*}", RegexOptions.Compiled);

        public static bool logSerialProcesses = false;

        /// <summary>
        /// Inludes the unity datapath.
        /// </summary>
        private static string gameDataDirectory;

        /// <summary>
        /// Sets the active path for the serializer.
        /// </summary>
        /// <param name="path"></param>
        public static void SetPath (string path) {
            gameDataDirectory = path;
            App.Current.LogHandler.Log ($"[DATA] Set serializer path: {path}");
        }

        /// <summary>
        /// Ensures the game data and its sub-folders are present. If not present, creates folders
        /// and returns false.
        /// </summary>
        /// <returns></returns>
        public static bool ValidateGameData () {
            string profileDirectory = $"{gameDataDirectory}/profiles";
            bool result = true;

            App.Current.LogHandler.StartBlock ($"[PATH] Path dump:");
            App.Current.LogHandler.Log ($"[PATH] {gameDataDirectory}");
            App.Current.LogHandler.Log ($"[PATH] {profileDirectory}");
            App.Current.LogHandler.Log ($"[PATH] {gameDataDirectory}/{DataHandler.SESSION_FILENAME}");

            if (Directory.Exists (gameDataDirectory) == false) {
                App.Current.LogHandler.Log ($"[PATH/ERROR] Failed to locate root directory: {gameDataDirectory}");
                Directory.CreateDirectory (gameDataDirectory);
                App.Current.LogHandler.Log ($"[PATH] Created directory");
                result = false;
            }

            if (Directory.Exists (profileDirectory) == false) {
                App.Current.LogHandler.Log ($"[PATH/ERROR] Failed to locate profile directory: {profileDirectory}. \n\tCreating new.");
                Directory.CreateDirectory ($"{profileDirectory}");
                App.Current.LogHandler.Log ($"[PATH] Createdd directory");
                result = false;
            }

            if (File.Exists ($"{gameDataDirectory}/{DataHandler.SESSION_FILENAME}") == false) {
                App.Current.LogHandler.Log ($"[PATH/ERROR] Failed to locate session file: {$"{gameDataDirectory}/{DataHandler.SESSION_FILENAME}"}. \n\tCreating new.");
                File.Create ($"{gameDataDirectory}/{DataHandler.SESSION_FILENAME}");
                App.Current.LogHandler.Log ($"[PATH] Created directory and file");
                result = false;
            }
            App.Current.LogHandler.EndBlock ();


            return result;
        }

        /// <summary>
        /// Reads a .kda file into data blocks.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static KeyedDataBlock ReadData (string path) {


            Stack<KeyedDataBlock> blocks = new Stack<KeyedDataBlock> ();
            blocks.Push (KeyedDataBlock.Create ());
            int indentCount = 0;

            blocks.Peek ().data.Add ("path", path);

            //read lines fo the file
            foreach (var line in File.ReadLines (path)) {

                //Check for block starts and ends
                var match = ListStartRegex.Match (line);
                if (match.Success) {
                    string blockName = match.ToString ()[..^2];
                    var newBlock = KeyedDataBlock.Create ();

                    blocks.Peek ().blocks.Add (blockName, newBlock);
                    blocks.Push (newBlock);
                    continue;
                }
                else if (ListEndRegex.Match (line).Success) {
                    blocks.Pop ();
                    if (logSerialProcesses) {
                        indentCount--;
                    }
                    continue;
                }

                var keyValueMatch = KeyValueRegex.Match (line);
                if (keyValueMatch.Success) {

                    var key = keyValueMatch.Groups[1].Value.Trim ().ToLower ();
                    var value = keyValueMatch.Groups[2].Value.Trim ();

                    blocks.Peek ().data.Add (key, value);
                }
            }

            if (blocks.Count == 1) {
                return blocks.Pop ();
            }
            else {
                App.Current.LogHandler.Log ($"[DATA/ERROR]: Block Mismatch ({blocks.Count})! File cannot be read properly.");
                var block = blocks.Pop ();
                block.errorFlag = true;
                return block;
            }
        }

        /// <summary>
        /// Loads and returns the compress
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture2D ReadTexture (string path) {
            var image = new Texture2D (2, 2, TextureFormat.RGB565, false);
            var bytes = File.ReadAllBytes (path);
            image.LoadImage (bytes);
            image.EncodeToJPG ();
            return image;
        }

        /// <summary>
        /// Writes a texture to file.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="path"></param>
        public static void WriteTexture (Texture2D texture, string path) {
            byte[] bytes = texture.GetRawTextureData ();
            File.WriteAllBytes (path, bytes);
            if (logSerialProcesses) {
                App.Current.LogHandler.Space ();
                App.Current.LogHandler.Log ($"[DATA] Writing texture file: {path}" +
                    $"\n\t{texture.width} x {texture.height} px" +
                    $"\n\t{bytes.Length:N0} bytes");
            }
        }

        /// <summary>
        /// Writes a datablock to a .kda file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dataBlock"></param>
        public static void WriteData (string path, KeyedDataBlock dataBlock) {
            var sb = new StringBuilder ();
            if (logSerialProcesses) {
                App.Current.LogHandler.Space ();
                App.Current.LogHandler.Log ($"[DATA] Writing data file {Path.GetFileName (path)}");
            }

            WriteDataToStringBuilder (dataBlock, sb, 0);
            File.WriteAllText (path, sb.ToString ());
        }

        public static bool ValidateDirectory (string path, bool createIfMissing = false) {
            bool _ = Directory.Exists (path);
            if (!_ && createIfMissing) {
                Directory.CreateDirectory (path);
            }
            return _;
        }

        public static bool ValidateFile (string path, bool createIfMissing = false) {
            bool _ = File.Exists (path);
            if (!_ && createIfMissing) {
                File.Create (path);
            }
            return _;
        }

        /// <summary>
        /// Recursively writes datablocks to text.
        /// </summary>
        private static void WriteDataToStringBuilder (KeyedDataBlock dataBlock, StringBuilder stringBuilder, int indentLevel) {
            // Create indentation based on the current level in the hierarchy
            string indent = new string ('\t', indentLevel);

            // Write each key-value pair in the data dictionary
            foreach (var keyValue in dataBlock.data) {
                stringBuilder.AppendLine ($"{indent}{keyValue.Key}: {FormatForKDA (keyValue.Value)}");
                if (logSerialProcesses) App.Current.LogHandler.Append ($"\n{indent}{keyValue.Key} : {keyValue.Value}\n");
            }

            // Recursively write each nested block
            foreach (var block in dataBlock.blocks) {
                stringBuilder.AppendLine ($"{indent}{block.Key} {{");
                if (logSerialProcesses) App.Current.LogHandler.Append ($"\n{indent}block : {block.Key}\n");
                WriteDataToStringBuilder (block.Value, stringBuilder, indentLevel + 1);
                stringBuilder.AppendLine ($"{indent}}}");
            }
        }

        public static void WriteDataToLog (KeyedDataBlock dataBlock, string prefix = "") {

            // Write each key-value pair in the data dictionary
            foreach (var keyValue in dataBlock.data) {
                App.Current.LogHandler.Log ($"[DATA] /{prefix}{(string.IsNullOrEmpty (prefix) ? "" : "/")}{keyValue.Key}: {FormatForKDA (keyValue.Value)}");
            }

            // Recursively write each nested block
            foreach (var block in dataBlock.blocks) {
                string newPrefix = $"{prefix}{(string.IsNullOrEmpty (prefix) ? "" : "/")}{block.Key}";
                WriteDataToLog (block.Value, newPrefix);
            }
        }

        /// <summary>
        /// Format the value depending on its type for easier reading in the .kda file.
        /// </summary>
        private static string FormatForKDA (object value) {
            if (value is Color color) {
                return $"{color.r}, {color.g}, {color.b}, {color.a}";
            }
            if (value == null) {
                return "null";
            }
            return value.ToString ();
        }
    }
}