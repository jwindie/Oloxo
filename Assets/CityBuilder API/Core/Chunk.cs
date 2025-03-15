using UnityEngine;
using Citybuilder.Roads;
using Citybuilder.ProceduralGeometry;

namespace Citybuilder.Core {
    public class Chunk : MonoBehaviour {

        public static float CHUNK_EDGE_DEPTH = 4f;


        public enum GeometryLayer {
            All,
            Terrain,
            Clip,
            Structure,
            Foliage,
            Road
        }

        private readonly Vector3[] cornerOffsets = new Vector3[] {
            Vector3.left,
            Vector3.zero,
            Vector3.back,
            new Vector3 (-1,0,-1)
        };

        public static RoadSolverData roadSolverData;
        public static TerrainSolverData terrainSolverData;

        public int x { get; private set; }
        public int z { get; private set; }

        [SerializeField] private Tile[,] tiles;

        [SerializeField] private ChunkMesh terrainGeometry;
        [SerializeField] private ChunkMesh clipGeometry;
        [SerializeField] private ChunkMesh foliageGeometry;
        [SerializeField] private ChunkMesh roadGeometry;

        private bool drawTerrain = false;
        private bool drawRoad = false;
        private bool drawClip = false;
        private bool drawFoliage = false;

        private Vector3 chunkOrigin;
        private GeometryLayer geometryLayerForUpdate = GeometryLayer.All;

        //bool array for the various terrain layers

        /// <summary>
        /// Creates the chunk and tiles using the chuck coordinates.
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cz"></param>
        public void Init (int cx, int cz) {
            x = cx;
            z = cz;

            tiles = new Tile[World.Current.ChunkSize, World.Current.ChunkSize];

            chunkOrigin = new Vector3 (x, 0, z) * World.Current.ChunkSize;

            //set up the chunk meshes
            terrainGeometry.Init ();
            clipGeometry.Init ();
            foliageGeometry.Init ();
            roadGeometry.Init ();
        }

        public void InsertTile (Tile tile, int x, int z) {
            tiles[x, z] = tile;
        }

        public void RefreshAll () {
            drawTerrain = true;
            drawClip = true;
            drawFoliage = true;
            drawRoad = true;

            enabled = true;
        }

        public void RefreshTerrain () {
            drawTerrain = true;
            enabled = true;
        }

        public void RefreshRoads () {
            drawRoad = true;
            enabled = true;
        }

        public void RefreshClip () {
            drawClip = true;
            enabled = true;
        }

        public void RefreshFoliage () {
            drawFoliage = true;
            enabled = true;
        }

        private void LateUpdate () {

            if (drawTerrain) terrainGeometry.Clear ();
            if (drawClip) clipGeometry.Clear ();
            if (drawFoliage) foliageGeometry.Clear ();
            if (drawRoad) roadGeometry.Clear ();

            //for each tile
            for (int iz = 0 ; iz < World.Current.ChunkSize ; iz++) {

                for (int ix = 0 ; ix < World.Current.ChunkSize ; ix++) {

                    if (drawTerrain) {
                        terrainSolverData = new TerrainSolverData ();

                        for (int i = 0 ; i < 4 ; i++) {
                            Tile t = World.Current.TileGrid.GetValue (tiles[ix, iz].WorldPosition + cornerOffsets[i]);
                            if (t != null) {
                                terrainSolverData.terrainTypes[i] = t.TerrainType;
                            }
                        }

                        //wrote this at 1AM so its kinda meh
                        //connects tiles to the edge of the map
                        if (World.Current.ConnectToMapEdge) {

                            //if we are on the left ends of the map, copy the right side over to conqtinue an edge
                            if (tiles[ix, iz].WorldPosition == Vector3.zero) {
                                terrainSolverData.terrainTypes[0] = terrainSolverData.terrainTypes[1];
                                terrainSolverData.terrainTypes[2] = terrainSolverData.terrainTypes[1];
                                terrainSolverData.terrainTypes[3] = terrainSolverData.terrainTypes[1];
                            }
                            else if (tiles[ix, iz].WorldPosition.x == 0) {
                                terrainSolverData.terrainTypes[0] = terrainSolverData.terrainTypes[1];
                                terrainSolverData.terrainTypes[3] = terrainSolverData.terrainTypes[2];
                            }
                            else if (tiles[ix, iz].WorldPosition.z == 0) {
                                terrainSolverData.terrainTypes[3] = terrainSolverData.terrainTypes[0];
                                terrainSolverData.terrainTypes[2] = terrainSolverData.terrainTypes[1];
                            }
                        }

                        //render tile corner
                        Vector3 truePostiion = tiles[ix, iz].WorldPosition - chunkOrigin + new Vector3 (-.5f, 0, -.5f);

                        //render cliffs for min borders
                        if (tiles[ix, iz].WorldPosition.x == 0 || tiles[ix, iz].WorldPosition.z == 0) {
                            //draw terrain with sides
                            TerrainSolver.GenerateMesh (terrainGeometry, terrainSolverData, truePostiion, true);
                            //terrainGeometry.AddMesh (tiles[ix, iz].WorldPosition - chunkOrigin + new Vector3 (-.5f, 0, -.5f), 1, tiles[ix, iz].RandomIndexedRotation * 90, MeshLibrary.GetMesh ("terrain/rock_edge"));
                        }
                        else {
                            //draw terrain without sides
                            TerrainSolver.GenerateMesh (terrainGeometry, terrainSolverData, truePostiion, false);
                        }

                        //harbor is considered terrain
                        if (tiles[ix, iz].HarborState) {
                            terrainGeometry.AddMesh (tiles[ix, iz].WorldPosition - chunkOrigin, 1, 0, MeshLibrary.GetMesh ("terrain/dock"));
                        }
                        //tile must be land and have no roads
                        else if (tiles[ix, iz].TerrainType == TerrainType.Land && tiles[ix, iz].RoadType == RoadType.None) {
                            terrainGeometry.AddMesh (tiles[ix, iz].WorldPosition - chunkOrigin, 1, tiles[ix, iz].RandomIndexedRotation * 90, MeshLibrary.GetMesh ("models/grass"));
                        }
                    }

                    if (drawClip) {
                        if (tiles[ix, iz].HollowState) {
                            clipGeometry.AddQuadOnTile (tiles[ix, iz].WorldPosition - chunkOrigin);
                        }
                    }

                    if (drawFoliage) {
                        if (tiles[ix, iz].TreeState == 1) {//trees
                            float scale = NoiseGeneration.SampleNoise ("TreeSize", tiles[ix, iz].WorldPosition.x, tiles[ix, iz].WorldPosition.z);
                            //float scale = 1;
                            foliageGeometry.AddMesh (tiles[ix, iz].WorldPosition - chunkOrigin, scale, 0, MeshLibrary.GetMesh ("test/block_tree"));
                        }
                    }

                    if (drawRoad) {
                        if (tiles[ix, iz].RoadType > RoadType.None) {
                            roadGeometry.AddQuadOnTile (tiles[ix, iz].WorldPosition - chunkOrigin);
                            roadGeometry.AddQuadColor (Color.gray);
                        }
                    }
                }
            }

            if (drawTerrain) {
                //terrain meshing for edges of chunks
                if (geometryLayerForUpdate == GeometryLayer.All || geometryLayerForUpdate == GeometryLayer.Terrain) {

                    //render terrain at world boundaries
                    if (x == World.Current.WorldSizeInChunks - 1) {
                        //x only
                        for (int iz = 0 ; iz < World.Current.ChunkSize ; iz++) {


                            terrainSolverData = new TerrainSolverData ();
                            //analyze the tile and its terrain type neighbors into a terrainCorner
                            Vector3 position = chunkOrigin + new Vector3 (World.Current.ChunkSize, 0, iz);
                            for (int i = 0 ; i < 4 ; i++) {
                                Tile t = World.Current.TileGrid.GetValue (position + cornerOffsets[i]);
                                if (t != null) terrainSolverData.terrainTypes[i] = t.TerrainType;
                            }

                            //connect edges
                            if (World.Current.ConnectToMapEdge) {
                                if (position.z == 0) {
                                    terrainSolverData.terrainTypes[1] =
                                    terrainSolverData.terrainTypes[2] =
                                    terrainSolverData.terrainTypes[3] = terrainSolverData.terrainTypes[0];
                                }
                                else {
                                    terrainSolverData.terrainTypes[1] = terrainSolverData.terrainTypes[0];
                                    terrainSolverData.terrainTypes[2] = terrainSolverData.terrainTypes[3];
                                }
                            }

                            TerrainSolver.GenerateMesh (terrainGeometry, terrainSolverData, position - chunkOrigin + new Vector3 (-.5f, 0, -.5f), true);
                            Tile _ = tiles[World.Current.ChunkSize - 1, iz];
                            //terrainGeometry.AddMesh (_.WorldPosition - chunkOrigin + new Vector3 (.5f, 0, -.5f), 1, _.RandomIndexedRotation * 90, MeshLibrary.GetMesh ("terrain/rock_edge"));
                        }
                        if (z == World.Current.WorldSizeInChunks - 1) {
                            //x and z
                            Vector3 position = chunkOrigin + new Vector3 (World.Current.ChunkSize, 0, World.Current.ChunkSize);
                            for (int i = 0 ; i < 4 ; i++) {
                                Tile t = World.Current.TileGrid.GetValue (position + cornerOffsets[i]);
                                if (t != null) terrainSolverData.terrainTypes[i] = t.TerrainType;

                            }

                            if (World.Current.ConnectToMapEdge) {
                                //connect edges
                                terrainSolverData.terrainTypes[0] =
                                terrainSolverData.terrainTypes[1] =
                                terrainSolverData.terrainTypes[2] = terrainSolverData.terrainTypes[3];
                            }

                            TerrainSolver.GenerateMesh (terrainGeometry, terrainSolverData, position - chunkOrigin + new Vector3 (-.5f, 0, -.5f), true);
                            Tile _ = tiles[World.Current.ChunkSize - 1, World.Current.ChunkSize - 1];
                            //terrainGeometry.AddMesh (_.WorldPosition - chunkOrigin + new Vector3 (.5f, 0, +.5f), 1, _.RandomIndexedRotation * 90, MeshLibrary.GetMesh ("terrain/rock_edge"));
                        }
                    }
                    if (z == World.Current.WorldSizeInChunks - 1) {
                        //z only
                        for (int ix = 0 ; ix < World.Current.ChunkSize ; ix++) {

                            //analyze the tile and its terrain type neighbors into a terrainCorner
                            Vector3 position = chunkOrigin + new Vector3 (ix, 0, World.Current.ChunkSize);
                            for (int i = 0 ; i < 4 ; i++) {
                                Tile t = World.Current.TileGrid.GetValue (position + cornerOffsets[i]);
                                if (t != null) terrainSolverData.terrainTypes[i] = t.TerrainType;
                            }

                            if (World.Current.ConnectToMapEdge) {
                                //connect edges
                                if (position.x == 0) {
                                    terrainSolverData.terrainTypes[0] =
                                    terrainSolverData.terrainTypes[1] =
                                    terrainSolverData.terrainTypes[3] = terrainSolverData.terrainTypes[2];
                                }
                                else {
                                    terrainSolverData.terrainTypes[0] = terrainSolverData.terrainTypes[3];
                                    terrainSolverData.terrainTypes[1] = terrainSolverData.terrainTypes[2];
                                }
                            }

                            TerrainSolver.GenerateMesh (terrainGeometry, terrainSolverData, position - chunkOrigin + new Vector3 (-.5f, 0, -.5f), true);
                            Tile _ = tiles[ix, World.Current.ChunkSize - 1];
                            //terrainGeometry.AddMesh (_.WorldPosition - chunkOrigin + new Vector3 (-.5f, 0, .5f), 1, _.RandomIndexedRotation * 90, MeshLibrary.GetMesh ("terrain/rock_edge"));
                        }
                    }
                }
            }

            //finalize the meshes
            if (drawTerrain) terrainGeometry.Apply (false);
            if (drawClip) clipGeometry.Apply (false);
            if (drawFoliage) foliageGeometry.Apply (true);
            if (drawRoad) roadGeometry.Apply (false);

            drawTerrain = false;
            drawClip = false;
            drawFoliage = false;
            drawRoad = false;

            enabled = false;
        }
    }
}