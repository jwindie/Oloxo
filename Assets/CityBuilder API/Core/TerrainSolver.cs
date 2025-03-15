
using Citybuilder.ProceduralGeometry;
using UnityEngine;

namespace Citybuilder.Core {

    public class TerrainSolverData {
        public TerrainType[] terrainTypes = new TerrainType[4];
        public float rotation;

        public TerrainSolverData (TerrainType defaultType = TerrainType.Water) {
            for (int i = 0 ; i < terrainTypes.Length ; i++) terrainTypes[i] = defaultType;
        }
    }
    public static class TerrainSolver {

        //private static Mesh land;
        //private static Mesh land_corner;
        //private static Mesh land_corner_double;
        //private static Mesh land_edge;
        //private static Mesh water;
        //private static Mesh water_corner;

        static TerrainSolver () {

            //load all terrain meshes into memory for filtering
            MeshLibrary.AddMeshesFromResources ("terrain");
            //land = Resources.Load<GameObject> ("Terrain/land").GetComponent<MeshFilter> ().sharedMesh;
            //land_corner = Resources.Load<GameObject> ("Terrain/land_corner").GetComponent<MeshFilter> ().sharedMesh;
            //land_corner_double = Resources.Load<GameObject> ("Terrain/land_corner_double").GetComponent<MeshFilter> ().sharedMesh;
            //land_edge = Resources.Load<GameObject> ("Terrain/land_edge").GetComponent<MeshFilter> ().sharedMesh;
            //water = Resources.Load<GameObject> ("Terrain/water").GetComponent<MeshFilter> ().sharedMesh;
            //water_corner = Resources.Load<GameObject> ("Terrain/water_corner").GetComponent<MeshFilter> ().sharedMesh;
        }

        public static void GenerateMesh (ChunkMesh chunkMesh, TerrainSolverData data, Vector3 position, bool drawTerainSides) {
            int landCorners = 0;
            int waterCorners = 0;

            for (int i = 0 ; i < 4 ; i++) {
                if (data.terrainTypes[i] == TerrainType.Land) landCorners++;
                else waterCorners++;
            }

            //water and land tiles are the easiest
            int rotation = 0;
            if (waterCorners == 4) chunkMesh.AddMesh (position, 1, rotation, MeshLibrary.GetMesh (drawTerainSides ? "terrain/water" : "terrain/water_top"));
            if (landCorners == 4) chunkMesh.AddMesh (position, 1, rotation, MeshLibrary.GetMesh (drawTerainSides ? "terrain/land" : "terrain/land_top"));

            //do corners
            if (landCorners == 1) {
                //iterate for each corner
                for (int i = 0 ; i < 4 ; i++) {

                    //check for correct orientation
                    if (data.terrainTypes[i] == TerrainType.Land) {
                        chunkMesh.AddMesh (position, 1, rotation, MeshLibrary.GetMesh (drawTerainSides ? "terrain/land_corner": "terrain/land_corner_top"));
                    }
                    rotation += 90;
                }
            }
            else if (waterCorners == 1) {
                //iterate for each corner
                for (int i = 0 ; i < 4 ; i++) {

                    //check for correct orientation
                    if (data.terrainTypes[i] == TerrainType.Water) {
                        chunkMesh.AddMesh (position, 1, rotation, MeshLibrary.GetMesh (drawTerainSides ? "terrain/water_corner" : "terrain/water_corner_top"));
                    }
                    rotation += 90;
                }
            }

            //do edges
            if (landCorners == 2) {
                for (int i = 0 ; i < 4 ; i++) {
                    //check for correct orientation
                    if (data.terrainTypes[i] == TerrainType.Land && data.terrainTypes[WrapIndex (i + 1)] == TerrainType.Land) {
                        chunkMesh.AddMesh (position, 1, rotation, MeshLibrary.GetMesh (drawTerainSides ? "terrain/land_edge" : "terrain/land_edge_top"));
                    }
                    rotation += 90;
                }

                //if none of these, we may have a double corner
                for (int i = 0 ; i < 4 ; i++) {
                    //check for correct orientation
                    if (data.terrainTypes[i] == TerrainType.Land && data.terrainTypes[WrapIndex (i + 2)] == TerrainType.Land) {

                        chunkMesh.AddMesh (position, 1, rotation, MeshLibrary.GetMesh (drawTerainSides ? "terrain/land_corner_double" : "terrain/land_corner_double_top"));
                    }
                    rotation += 90;
                }
            }

            ////generate rock edges for edge of map using position and world coordinates
            //if (position.x == -.5f ||
            //    position.x == World.Current.ChunkSize * World.Current.WorldSizeInChunks + .5f ||
            //    position.z == -.5f ||
            //    position.z == World.Current.ChunkSize * World.Current.WorldSizeInChunks + .5f) {
            //    chunkMesh.AddMesh (position, 1, 0, MeshLibrary.GetMesh ("terrain/rock_edge"));
            //}
        }

        private static int WrapIndex (int input) {
            if (input > 3) input -= 4;
            if (input < 0) input += 4;
            return input;
        }
    }
}
