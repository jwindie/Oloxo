using Oloxo.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oloxo.Data {
    [System.Serializable]
    public struct KeyedDataBlock {

        public Dictionary<string, string> data;
        public Dictionary<string, KeyedDataBlock> blocks;
        public bool errorFlag;

        /// <summary>
        /// Creates a new <see cref="KeyedDataBlock"/> ready for population.
        /// </summary>
        /// <returns></returns>
        public static KeyedDataBlock Create () {
            return new KeyedDataBlock {
                data = new Dictionary<string, string> (),
                blocks = new Dictionary<string, KeyedDataBlock> ()
            };
        }

        public static string ToString (int value) {
            return value.ToString ();
        }
        public static string ToString (float value) {
            return value.ToString ("N0");
        }
        public static string ToString (bool value) {
            return value ? "true" : "false";
        }
        public static string ToString (Vector2 value) {
            return $"{ToString (value[0])}, {ToString (value[1])}";
        }
        public static string ToString (Vector3 value) {
            return $"{ToString (value[0])}, {ToString (value[1])}, {ToString (value[2])}";
        }
        public static string ToString (Vector4 value) {
            return $"{ToString (value[0])}, {ToString (value[1])}, {ToString (value[2])}, {ToString (value[3])}";
        }
        public static string ToString (float[] values) {
            StringBuilder _ = new StringBuilder (ToString (values[0]));
            for (int i = 1 ; i < values.Length ; i++) _.Append ($", {ToString (values[0])}");
            return _.ToString ();
        }
        public static string ToString (int[] values) {
            StringBuilder _ = new StringBuilder (ToString (values[0]));
            for (int i = 1 ; i < values.Length ; i++) _.Append ($", {ToString (values[0])}");
            return _.ToString ();
        }
        public static string ToString (bool[] values) {
            StringBuilder _ = new StringBuilder (ToString (values[0]));
            for (int i = 1 ; i < values.Length ; i++) _.Append ($", {ToString (values[0])}");
            return _.ToString ();
        }
        public string GetString (string key) {
            if (data.ContainsKey (key)) return data[key];
            return string.Empty;

        }
        public int GetInt (string key) {
            int value = 0;
            if (data.ContainsKey (key)) {
                try {
                    int.TryParse (key, out value);
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return value;
        }
        public float GetFloat (string key) {
            float value = 0;
            if (data.ContainsKey (key)) {
                try {
                    float.TryParse (key, out value);
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return value;
        }
        public bool GetBool (string key) {
            bool value = false;
            if (data.ContainsKey (key)) {
                try {
                    bool.TryParse (key, out value);
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return value;
        }
        public string[] GetStringArray (string key, params char[] separator) {
            if (data.ContainsKey (key)) {
                string[] _ = data[key].Split (separator);
                _.Select (x => x.Trim ());
                return _;
            }
            return new string[0];
        }
        public float[] GetFloats (string key) {
            if (data.ContainsKey (key)) {
                try {
                    return GetStringArray (key, ',').Select (x => float.Parse (x)).ToArray ();
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return new float[0];
        }
        public int[] GetInts (string key) {
            if (data.ContainsKey (key)) {
                try {
                    return GetStringArray (key, ',').Select (x => int.Parse (x)).ToArray ();
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return new int[0];
        }
        public bool[] GetBools (string key) {
            if (data.ContainsKey (key)) {
                try {
                    return GetStringArray (key, ',').Select (x => bool.Parse (x)).ToArray ();
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return new bool[0];
        }
        public Color GetColor (string key) {
            if (data.ContainsKey (key)) {
                try {
                    var _ = GetStringArray (key, ',').Select (x => float.Parse (x)).ToArray ();
                    return new Color (_[0], _[1], _[2], _[3]);
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return Color.black;
        }
        public Vector2 GetVector2 (string key) {
            if (data.ContainsKey (key)) {
                try {
                    var _ = GetStringArray (key, ',').Select (x => float.Parse (x)).ToArray ();
                    return new Vector2 (_[0], _[1]);
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return Vector2.zero;
        }
        public Vector3 GetVector3 (string key) {
            if (data.ContainsKey (key)) {

                try {
                    var _ = GetStringArray (key, ',').Select (x => float.Parse (x)).ToArray ();
                    return new Vector3 (_[0], _[1], _[2]);
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return Vector3.zero;
        }
        public Vector4 GetVector4 (string key) {
            if (data.ContainsKey (key)) {

                try {
                    var _ = GetStringArray (key, ',').Select (x => float.Parse (x)).ToArray ();
                    return new Vector4 (_[0], _[1], _[2], _[3]);
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return Vector4.zero;
        }
        public DateTime GetDate (string key) {
            if (data.ContainsKey (key)) {
                try {
                    return DateTime.Parse (data[key]);
                }
                catch {
                    //any error on parse gets logged 
                    App.Current.LogHandler.Log ($"[DATA/ERROR] Failed to parse: {Path.GetDirectoryName (data["path"])}/{key}");
                }
            }
            return DateTime.MinValue;
        }
    }
}