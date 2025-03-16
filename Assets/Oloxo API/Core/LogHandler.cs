using System;
using System.IO;
using UnityEngine;

namespace Oloxo.Core {
    /// <summary>
    /// Writes to a log file for debugging and error reporting.
    /// </summary>
    public class LogHandler {

        private bool streamOpen;
        private StreamWriter stream;
        private bool logToUnity = false;
        private bool inBlock = false;

        public LogHandler (string path) {
            NewStream (path);
        }

        /// <summary>
        /// Returns the current time in a timestamp.
        /// </summary>
        /// <returns></returns>
        public static string GetTimestamp () {
            return $"[{DateTime.Now.ToString ("hh:mm:ss")}]";
        }

        /// <summary>
        /// Logs a message to the logfile with a timestamp.
        /// </summary>
        /// <param name="message"></param>
        public void Log (string message) {
            if (!inBlock) {
                if (streamOpen) {
                    stream.WriteLine ($"{GetTimestamp ()} {message}");
                }
                if (logToUnity) Debug.Log (message);
            }
            else LogNTS (message);
        }

        /// <summary>
        /// Staets a formatting block for logs. All subsequent loogs will use NTS.
        /// </summary>
        public void StartBlock (string message) {
            if (streamOpen) {
                stream.WriteLine ($"{GetTimestamp ()} {message}");
            }
            if (logToUnity) Debug.Log (message);
            inBlock = true;
        }

        public void EndBlock () {
            inBlock = false;
            if (streamOpen) {
                stream.WriteLine ();
            }
        }

        public void LogDivider (int length = 30) {
            App.Current.LogHandler.LogNTS (new string ('*', length));
        }

        /// <summary>
        /// Appends the logfile. Does not include a timestamp.
        /// </summary>
        /// <param name="message"></param>
        public void Append (string message) {
            if (streamOpen) {
                stream.Write (message);
            }
        }

        /// <summary>
        /// Writes an empty line to the console.
        /// </summary>
        public void Space (int count = 1) {
            if (streamOpen) {
                stream.Write (new string ('\n', count));
            }
        }

        /// <summary>
        /// Optional enable to log to unity as well. Note that all logs are 
        /// basic unity messages. Errors and warnings are not supported.
        /// </summary>
        /// <param name="state"></param>
        public void LogToUnity (bool state) {
            logToUnity = state;
        }

        /// <summary>
        /// Closes and flushes the log file.
        /// </summary>
        public void Close () {
            if (streamOpen) {
                stream.Flush ();
                stream.Close ();
                streamOpen = false;
            }
        }

        /// <summary>
        /// Creates a new stream at the path. Returns 
        /// false if the path did not already exist.
        /// </summary>
        /// <param name="path"></param>
        private bool NewStream (string path) {
            Close ();

            bool result = false;

            //open a new stream
            if (ValidatePath (path) == false) {
                //create the directory
                Directory.CreateDirectory (path);
            }
            else result = true;

            stream = new StreamWriter ($"{path}/latest.log");
            stream.AutoFlush = true;
            streamOpen = true;

            return result;
        }

        private bool ValidatePath (string path) {
            return Directory.Exists (path);
        }

        /// <summary>
        /// Logs a message to the logfile without a timestamp
        /// </summary>
        /// <param name="message"></param>
        private void LogNTS (string message) {
            if (streamOpen) {
                string indent = GetTimestamp ();
                indent = new string (' ', indent.Length);
                stream.WriteLine ($"{indent} {message}");
            }
            if (logToUnity) Debug.Log (message);
        }
    }
}