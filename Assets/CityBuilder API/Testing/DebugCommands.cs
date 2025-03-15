using Citybuilder.Core;
using JWIndie.RuntimeDebugger;
using UnityEngine;

namespace Citybuilder.Testing {
    public class DebugCommands : MonoBehaviour {
        private void Start () {

            RuntimeDebugController.AddCommand (new RuntimeDebugCommand (
                "worldsize",
                "gets the world size",
                "worldsize",
                () => {
                    Debug.Log ($"World Size: C {World.Current.WorldSizeInChunks}" +
                        $"/T {World.Current.WorldSizeInTiles}" +
                        $"/A {World.Current.WorldSizeInTiles * World.Current.WorldSizeInTiles}");
                }
                ));

            RuntimeDebugController.AddCommand (new RuntimeDebugCommand (
                "percentforest",
                "gets the percentage of forest in the map",
                "percentforest",
                () => {
                    float count = 0;
                    float total = 0;
                    foreach (Tile t in World.Current.AllTiles ()) {
                        if (t.TreeState > 0) count++;
                        if (t.TerrainType == TerrainType.Land) total++;
                    }
                    Debug.Log ($"World is {count / total * 100}% forest");
                }
                ));
            RuntimeDebugController.AddCommand (new RuntimeDebugCommand (
                "percentwater",
                "gets the percentage of water in the map",
                "percentwater",
                () => {
                    float count = 0;
                    float total = 0;
                    foreach (Tile t in World.Current.AllTiles ()) {
                        if (t.TerrainType == TerrainType.Water) count++;
                        total++;
                    }
                    Debug.Log ($"World is {count / total * 100}% water");
                }
                ));
        }
    }
}
