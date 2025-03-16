using Oloxo.Cameras;
using Oloxo.Core;
using Oloxo.Data;
using Oloxo.HexSystem;
using UnityEngine;

namespace Oloxo.Global {

    /// <summary>
    /// Handles most logic for the game including game states, managing other managers and simulation.
    /// </summary>
    public class GameManager {
        private GameState currentState;

        public enum GameState {
            Menu,
            World
        }

        public EventHandler EventHandler { get; private set; }
        public ResourceManager ResourceManager { get; private set; }
        public Models.ModelLoader ModelLoader { get; private set; }
        public HexGrid HexGrid { get; private set; }    
        public Cameras.SimpleCamera CameraController { get; private set; }    

        public GameManager () {
            EventHandler = new EventHandler ();
        }

        public void Initialize () {
            Serializer.ValidateGameData ();

            ResourceManager = Object.FindObjectOfType<ResourceManager> ();

            //load all models into the game
            //since we only do this once, we do not need to store a reference to the loader
            ModelLoader = App.Current.GetComponentInChildren<Models.ModelLoader> ().Init ();
            ModelLoader.Load ();

            CameraController = App.Current.GetComponentInChildren<SimpleCamera> ();

            //find and initialize the hexGrid
            HexGrid = App.Current.GetComponentInChildren<HexGrid> ();
            HexGrid.Init ();
        }

        ///// <summary>
        ///// Loads game data into the arrays.
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="profileData"></param>
        ///// <param name="profileTextures"></param>
        //private void LoadGameData (out KeyedDataBlock session, out KeyedDataBlock[] profileData, out Texture2D[] profileTextures) {


        //    App.Current.LogHandler.StartBlock ("[LOAD] Loading game data...");

        //    //find the folder for the session first
        //    if (Serializer.ValidateDirectory (DataHandler.SESSION_PATH) == false) {
        //        //there was no session data. what do?
        //        App.Current.LogHandler.Log ("[LOAD/WARN] Missing session data. Used default session");
        //    }
        //    else {
        //        //read the session data
        //        session = Serializer.ReadData (DataHandler.SESSION_PATH);
        //    }
        //    App.Current.LogHandler.Log ($"[LOAD] Reading {DataHandler.SESSION_FILENAME}");
        //    Serializer.WriteDataToLog (session, "session");

        //    //find all the textures
        //    string[] directories;
        //    if (Serializer.ValidateDirectory ($"{Application.dataPath}/Data/sample/profiles", true) == false) {
        //        //profiles dir not found
        //        App.Current.LogHandler.Log ("[LOAD] Could not locate profiles folder. Created one instead");
        //        directories = new string[0];
        //        profileData = new KeyedDataBlock[0];
        //        profileTextures = new Texture2D[0];
        //    }
        //    else {
        //        //profiles dir found
        //        directories = Directory.GetDirectories ($"{Application.dataPath}/Data/sample/profiles");
        //        profileData = new KeyedDataBlock[directories.Length];
        //        profileTextures = new Texture2D[directories.Length];
        //        App.Current.LogHandler.Log ($"[LOAD] Found {directories.Length} profiles");
        //        App.Current.LogHandler.LogDivider ();
        //    }

        //    //load all the directories into profiles
        //    for (int i = 0 ; i < directories.Length ; i++) {
        //        App.Current.LogHandler.Log ($"[LOAD] Profile {Path.GetFileName (directories[i])}:");

        //        //load data
        //        if (Serializer.ValidateFile ($"{directories[i]}/profile.kda")) {
        //            profileData[i] = Serializer.ReadData ($"{directories[i]}/profile.kda");
        //            profileData[i].data.Add ("id", Path.GetFileName (directories[i])); //add the id into the data
        //            App.Current.LogHandler.Log ($"[DATA] Data: Found!");
        //            Serializer.WriteDataToLog (profileData[i]);
        //        }
        //        else App.Current.LogHandler.Log ($"[LOAD] Data: Missing!");

        //        //load textures
        //        if (Serializer.ValidateFile ($"{directories[i]}/image.jpg")) {
        //            profileTextures[i] = Serializer.ReadTexture ($"{directories[i]}/image.jpg");
        //            App.Current.LogHandler.Log ($"[DATA] Texture: Found ({profileTextures[i].width.ToString ("N0")} x {profileTextures[i].height.ToString ("N0")}px)");
        //        }
        //        else App.Current.LogHandler.Log ($"[LOAD] Texture: Missing!");

        //        App.Current.LogHandler.LogDivider ();
        //    }


        //    App.Current.LogHandler.Log ("[LOAD] Done!");
        //    App.Current.LogHandler.EndBlock ();
        //}

        private void SetGameState (GameState state) {
            currentState = state;
            Core.App.Current.LogHandler.Log ($"Game state set to: {state}");

            //depending on the game mode, do something
        }
    }
}