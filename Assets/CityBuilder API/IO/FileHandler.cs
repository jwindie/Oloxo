using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Citybuilder.UI;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Citybuilder.IO {


    public static class FileHandler {

        public enum PathForSaveVerificationResult {
            Invalid,
            NotExist,
            Exist
        }

        public static string INVALID_SAVE_CHARACTERS_PATTERN = @".[\/\\!@#$%^&*()?><.,;':""|`~\[\]]";

        public static LoadModalOption.LoadModalOptionData[] GetLoadOptions () {
            //locate the saves folder and get all directories in that folder

            string[] directories = Directory.GetDirectories (Paths.WORLD_SAVE_PATH);

            if (directories.Length > 0) {
                //create list of data
                List<LoadModalOption.LoadModalOptionData> data = new List<LoadModalOption.LoadModalOptionData> ();

                for (int i = 0 ; i < directories.Length ; i++) {
                    //ensure the data files are present
                    if (!File.Exists ($"{directories[i]}/world.world")) continue;
                    if (!File.Exists ($"{directories[i]}/world.world")) continue;

                    //put properties into a  loadModal option

                    data.Add (ParseLoadModeOptionData (directories[i]));
                }

                return data.ToArray ();
            }
            else {
                return new LoadModalOption.LoadModalOptionData[0];
            }
        }

        public static int DeleteDirectory (string path) {
            if (Directory.Exists (path)) {

                //delete all files in the path
                foreach (string file in Directory.GetFiles (path)) {
                    File.Delete (file);
                }
                Directory.Delete (path);

                //after deleting check to see if it is deleted
                if (Directory.Exists (path)) {
                    //failed to delete
                    return 0;
                }
                else {
                    return 1; //success
                }
            }
            return -1; //no directory
        }

        private static LoadModalOption.LoadModalOptionData ParseLoadModeOptionData (string path) {
            LoadModalOption.LoadModalOptionData _ = new LoadModalOption.LoadModalOptionData ();

            //get name of the file 
            var dInfo = new DirectoryInfo (path);
            _.worldName = dInfo.Name;
            _.path = path;

            //parse the info.text file
            Dictionary<string, string> metaData = new Dictionary<string, string> ();
            using (StreamReader reader = new StreamReader ($"{path}/info.txt")) {
                //get the last component of each line trimmed
                string line = "";
                while ((line = reader.ReadLine ()) != null) {
                    string[] split = line.Trim ().Split ('=');
                    if (split.Length == 2) {
                        Debug.Log (split[0]);

                        metaData.Add (split[0].Trim(), split[1].Trim());
                    }
                }
            };
            _.metaData = metaData;

            //get image if it exists
            byte[] bytes = null;
            if (File.Exists ($"{path}/thumbnail.jpg")) {
                bytes = File.ReadAllBytes ($"{path}/thumbnail.jpg");
            }
            else if (File.Exists ($"{path}/thumbnail.jpeg")) {
                bytes = File.ReadAllBytes ($"{path}/thumbnail.jpeg");
            }
            else if (File.Exists ($"{path}/thumbnail.png")) {
                bytes = File.ReadAllBytes ($"{path}/thumbnail.png");
            }

            if (bytes != null) {
                var tex = new Texture2D (2, 2);
                tex.LoadImage (bytes);
                _.icon = tex;
            }

            //return the loadData
            return _;
        }

        public static PathForSaveVerificationResult VerifyPathForSave (string path) {

            if (path.Length < 1 || Regex.IsMatch (path, INVALID_SAVE_CHARACTERS_PATTERN)) {
                return PathForSaveVerificationResult.Invalid;
            }
            if (Directory.Exists ($"{Paths.WORLD_SAVE_PATH}/{path}")) {
                return PathForSaveVerificationResult.Exist;
            }
            return PathForSaveVerificationResult.NotExist;
        }
    }
}
