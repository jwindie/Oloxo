using JWIndie.Utilities;
using Pathfinding;
using System.Collections;
using UnityEngine;

namespace Citybuilder.Core {

    //main handler of the game and sole entry point into the program
    class GameManager : Singleton<GameManager> {

        public static FileLog fileLog;

        [SerializeField] private Camera menuCamera;
        [SerializeField] private float menuCamerRotation;
        [Space]
        [SerializeField] private Canvas HUD;
        [SerializeField] private Canvas Menu;
        [SerializeField] private Canvas GUI;
        [SerializeField] private AstarPath astarPath;
        [Space]
        [Header ("DEBUG ONLY")]
        [SerializeField] public Transform target;
        private bool menuMode = false;

        public FileLog FileLog {
            get {
                return fileLog;
            }
        }

        public bool IsMenuMode {
            get {
                return menuMode;
            }
        }

        private void Awake () {
            //Create fileLog in data folder
            fileLog = new FileLog ($"{IO.Paths.DATA_PATH}/log.txt", true);
            fileLog.EnableTimeLogging (false);
            fileLog.WriteLine ($"*** LOG FILE ({Application.productName} {Application.version})***");
            fileLog.Space ();


            //generate all noise values

            //validate singletons
            fileLog.Write ($"Validating singletons...");
            if (CameraController.Current == null) {
                fileLog.WriteLine ("Failed!");
                fileLog.WriteLine ($"\tFATAL: Missing camera controller!");
                Application.Quit ();
                return;
            }
            if (SettingsManager.Current == null) {
                fileLog.WriteLine ("Failed!");
                fileLog.WriteLine ($"\tFATAL: Missing settings manager!");
                Application.Quit ();
                return;
            }
            if (World.Current == null) {
                fileLog.WriteLine ("Failed!");
                fileLog.WriteLine ($"\tFATAL: Missing world!");
                Application.Quit ();
                return;
            }
            fileLog.WriteLine ("Success!");
            fileLog.Space ();

            //load assets
            fileLog.WriteLine ("Loading meshes...");
            MeshLibrary.AddMeshesFromResources ("test");
            MeshLibrary.AddMeshesFromResources ("models");

            Resources.UnloadUnusedAssets ();
            MeshLibrary.LogMeshes (fileLog);
            fileLog.WriteLine ("Done!");
            fileLog.Space ();

            //create the AI graph
            fileLog.WriteLine ("Creating AI Graph");
            var _ = World.Current.AstarPath.data.AddGraph (typeof (GridGraph)) as GridGraph;
            //setup graph properties
            _.neighbours = NumNeighbours.Four;
            _.collision.collisionCheck = false;
            _.collision.heightCheck = false;
            _.showMeshSurface = true;
            _.showNodeConnections = true;
            _.showMeshOutline = false;
        }

        private void Start () {

            //lighting and fog settings
            //handle this with the settings manager as it has all the refs required
            RenderSettings.fogStartDistance = 30;
            SettingsManager.Current.LoadSettings (IO.Serializer.DeserializeSettings ());

            //no grid
            UI.GridToggleButton.Current.ToggleGrid (false);

            //hide HUD
            HUD.enabled = false;

            //enable DOF
            if (SettingsManager.Current.Volume.profile.TryGet (out UnityEngine.Rendering.Universal.DepthOfField dof)) {
                dof.active = true;
            }

            //selector
            Selector.Current.Hide ();

            menuMode = true;

            //load a default map for the menu
            LoadMenuMap ();

            CameraController.Current.gameObject.SetActive (false);
            StartCoroutine (RotateMenuCamera ());
        }

        private void LoadMenuMap () {
            string randomMenuMapName = GetRandomMenuMap ();
            if (randomMenuMapName != null) {
                FileLog.WriteLine ($"Chose {randomMenuMapName} for the menu.");
                IO.Serializer.DeserializeMapExplicit (randomMenuMapName);
            }
            else {//if no map found, generate a new blank one
                FileLog.WriteLine ($"Found no menu maps. Creating blank 4x4.");
                World.Current.CreateWorld (4);
            }
            fileLog.Space ();
        }

        public void LoadMap (string mapName) {
            StopAllCoroutines ();

            //settings
            HUD.enabled = true;
            Menu.enabled = false;
            Selector.Current.Hide (false);
            CameraController.Current.gameObject.SetActive (true);
            menuCamera.enabled = false;
            if (SettingsManager.Current.Volume.profile.TryGet (out UnityEngine.Rendering.Universal.DepthOfField dof)) {
                dof.active = false;
            }
            Selector.Current.AreaDraggingEnabled = false;

            //generation
            bool loadedMap = false;
            if (mapName != null) {
                if (IO.Serializer.DeserializeMapExplicit (mapName)) loadedMap = true;
            }

            if (loadedMap == false) {
                //if no map found, generate a new blank one
                FileLog.WriteLine ($"Loading default map");
                World.Current.CreateWorld (4);
            }

            menuMode = false;
        }

        public void LoadMap (int size) {
            StopAllCoroutines ();

            //settings
            HUD.enabled = true;
            Menu.enabled = false;
            Selector.Current.Hide (false);
            CameraController.Current.gameObject.SetActive (true);
            menuCamera.enabled = false;
            if (SettingsManager.Current.Volume.profile.TryGet (out UnityEngine.Rendering.Universal.DepthOfField dof)) {
                dof.active = false;
            }
            Selector.Current.AreaDraggingEnabled = false;

            //generation
            World.Current.CreateWorld (size);

            menuMode = false;
        }

        public void LoadMenu () {
            StopAllCoroutines ();

            //settings
            HUD.enabled = false;
            Menu.enabled = true;
            Selector.Current.Hide ();
            menuCamera.enabled = true;
            CameraController.Current.gameObject.SetActive (false);
            if (SettingsManager.Current.Volume.profile.TryGet (out UnityEngine.Rendering.Universal.DepthOfField dof)) {
                dof.active = true;
            }
            UI.GridToggleButton.Current.ToggleGrid (false);

            //generation
            LoadMenuMap ();
            StartCoroutine (RotateMenuCamera ());

            menuMode = true;
        }

        private string GetRandomMenuMap () {
            var _ = IO.Serializer.GetMenuSaves ();
            if (_.Length == 0) return null;
            return _[Random.Range (0, _.Length)];
        }

        public void Quit () {
            fileLog.WriteLine ($"Quitting application");
            fileLog.Close ();
            Application.Quit ();
        }

        public void HideGUI (bool state) {
            GUI.enabled = !state;
        }

        private IEnumerator RotateMenuCamera () {
            Vector3 center = World.Current.Center;
            while (true) {
                menuCamera.transform.RotateAround (center, Vector3.up, menuCamerRotation * Time.deltaTime);
                yield return null;
            }
        }
    }
}
