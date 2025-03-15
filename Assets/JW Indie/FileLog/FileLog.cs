using UnityEngine;
using System;
using System.IO;

namespace JWIndie.Utilities {
    /// <summary>
    /// Easily accessible file logger for selective logging through Unity.
    /// </summary>
    public partial class FileLog {
        private bool
           autoFlush,
           streamOpen = false,
           timeLogEnabled = true;
        private string filePath;
        private StreamWriter stream;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch ();

        private string TimeLabel {
            get {
                return $"[{DateTime.Now.ToLongTimeString ()}]";
            }
        }

        public bool HasActiveStream {
            get {
                return (stream != null);
            }
        }

        public FileLog (string filePath, bool autoFlush) {
            NewStream (filePath);
            EnableAutoFlush (autoFlush);
        }

        /// <summary>
        /// Sets the file path
        /// </summary>
        /// <param name="path"></param>
        public void NewStream (string filepath) {
            if (streamOpen) {
                Debug.LogWarning ("Automatically closing the last stream.");
                stream.Flush ();
                stream.Close ();
            }
            //open new stream at the filePath
            try {
                stream = new StreamWriter (filePath = filepath);
                stream.AutoFlush = autoFlush;
                streamOpen = true;
            }
            catch (IOException e) {
                Debug.LogError ("Could not open new stream!");
                Debug.LogException (e);
                stream = null;
            }
        }

        public void EnableTimeLogging (bool state) {
            timeLogEnabled = state;
        }

        public void EnableAutoFlush (bool state) {
            autoFlush = state;
            if (stream != null) stream.AutoFlush = autoFlush;
        }

        public void OpenFile () {
            try {
                System.Diagnostics.Process.Start (filePath);
            }
            catch (InvalidOperationException) {
                Debug.LogError ("Failed to open the file!");
            }

        }

        public void StartProcessTimer () {
            stopwatch.Start ();
        }

        public float StopProessTimer () {
            stopwatch.Stop ();
            long elap = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset ();
            return elap / 1000f;
        }

        public void Close () {
            if (streamOpen) {
                stream.Flush ();
                stream.Close ();
                streamOpen = false;
            }
        }

        public void Write (string message, bool allowTimeStamp = true) {
            if (streamOpen) {
                if (allowTimeStamp && timeLogEnabled) {
                    stream.Write ($"{TimeLabel} {message}");
                }
                else {
                    stream.Write (message);
                }
            }
        }
        public void WriteLine (string message, bool allowTimeStamp = true) {
            if (streamOpen) {
                if (allowTimeStamp && timeLogEnabled) {
                    stream.WriteLine ($"{TimeLabel} {message}");
                }
                else {
                    stream.WriteLine (message);
                }
            }
        }

        public void Space () {
            if (streamOpen) {
                stream.WriteLine ();
            }
        }
    }
}