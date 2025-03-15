using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Core {
    public class World : Singleton<World> {

        private const int CHUNK_SIZE = 8;
        private static bool connectToMapEdge_lastValue;
        public GenericGrid<Tile> TileGrid { get; private set; }
        private Chunk[,] chunks;
        [SerializeField] GameObject chunkPrefab;
        [SerializeField] AstarPath astarPath;

        [SerializeField] private int worldSize;
        [SerializeField] private bool connectToMapEdge;
        [SerializeField] private TerrainType defaultTerrainType;
        [SerializeField] private bool drawGizmos;


        [Header ("Material Settings")]
        [SerializeField] private Material mainMaterial;
        [SerializeField] private float fogAmount;
        [SerializeField] private Color fogColor;

        [SerializeField] private NoiseGeneration.NoisePreset TreeNoise;
        [SerializeField] [Range (0, 3)] private int centerLandRadius;

        private bool hasWorld;
        public int ChunkSize {
            get {
                return CHUNK_SIZE;
            }
        }
        public int WorldSizeInChunks {
            get {
                return worldSize;
            }
        }
        public int WorldSizeInTiles {
            get {
                return worldSize * CHUNK_SIZE;
            }
        }
        public bool ConnectToMapEdge {
            get {
                return connectToMapEdge;
            }
        }

        public Vector3 Center {
            get {
                float center = worldSize * CHUNK_SIZE / 2f;
                return new Vector3 (center, 0, center);
            }
        }

        public AstarPath AstarPath {
            get {
                return astarPath;
            }
        }

        //Generates a world of chunks of a size
        public void CreateWorld (int worldSize, bool createCenterLand = true) {
            if (worldSize > 0) {

                if (hasWorld) {
                    GameManager.Current.FileLog.WriteLine ("Clearing old world.");

                    //are the new worlds the exact same size?
                    if (this.worldSize == worldSize) {
                        ClearMap ();
                        return;
                    }
                    else {
                        DeleteWorldWithAssets ();
                    }
                }

                this.worldSize = worldSize;
                int worldTileDimensions = CHUNK_SIZE * worldSize;

                TileGrid = new GenericGrid<Tile> (worldTileDimensions, worldTileDimensions, 1);

                #region Reformat AI Graph
                // Setup the grid graph with some values
                if (AstarData.active.data.gridGraph != null) {
                    var _ = AstarData.active.data.gridGraph;
                    _.SetDimensions (worldTileDimensions, worldTileDimensions, 1);

                    float half = worldTileDimensions / 2 - .5f;
                    _.center = new Vector3 (half, 0, half);
                    AstarData.active.Scan ();
                }
                #endregion

                //create chunks
                chunks = new Chunk[worldSize, worldSize];
                for (int z = 0 ; z < worldSize ; z++) {
                    for (int x = 0 ; x < worldSize ; x++) {
                        chunks[x, z] = CreateChunk (x, z);
                    }
                }

                //place tiles into the chunks
                var gg = AstarPath.active.data.gridGraph;

                for (int z = 0 ; z < worldTileDimensions ; z++) {
                    for (int x = 0 ; x < worldTileDimensions ; x++) {

                        int chunkCoordinateX = x / CHUNK_SIZE;
                        int chunkCoordinateZ = z / CHUNK_SIZE;

                        int coordinateInChunkX = x % CHUNK_SIZE;
                        int coordinateInChunkZ = z % CHUNK_SIZE;

                        //add chunks 
                        List<Chunk> tChunks = new List<Chunk> { chunks[chunkCoordinateX, chunkCoordinateZ] };

                        //max boundary x
                        if (coordinateInChunkX == CHUNK_SIZE - 1 && chunkCoordinateX < worldSize - 1) {
                            tChunks.Add (chunks[chunkCoordinateX + 1, chunkCoordinateZ]);

                            //max boundary z
                            if (coordinateInChunkZ == CHUNK_SIZE - 1 && chunkCoordinateZ < worldSize - 1) {
                                tChunks.Add (chunks[chunkCoordinateX + 1, chunkCoordinateZ + 1]);
                            }
                        }

                        //min boundary x
                        if (coordinateInChunkX == 0 && chunkCoordinateX > 0) {
                            tChunks.Add (chunks[chunkCoordinateX - 1, chunkCoordinateZ]);

                            //min boundary z
                            if (coordinateInChunkZ == 0 && chunkCoordinateZ > 0) {
                                tChunks.Add (chunks[chunkCoordinateX, chunkCoordinateZ - 1]);
                            }
                        }

                        //max boundary z
                        if (coordinateInChunkZ == CHUNK_SIZE - 1 && chunkCoordinateZ < worldSize - 1) {
                            tChunks.Add (chunks[chunkCoordinateX, chunkCoordinateZ + 1]);
                        }

                        //min boundary z
                        if (coordinateInChunkZ == 0 && chunkCoordinateZ > 0) {
                            tChunks.Add (chunks[chunkCoordinateX, chunkCoordinateZ - 1]);
                        }

                        //Debug.Log ($"{x}/{chunkCoordinateX}/{coordinateInChunkX}, {z}/{chunkCoordinateZ}/{coordinateInChunkZ}");
                        TileGrid[x, z] = new Tile (x, z, new Vector3 (x, 0, z), tChunks, defaultTerrainType);
                        chunks[chunkCoordinateX, chunkCoordinateZ].InsertTile (TileGrid.GetValue (x, z), coordinateInChunkX, coordinateInChunkZ);

                        var node = gg.GetNode (x, z);
                        node.Walkable = defaultTerrainType == TerrainType.Land;
                        gg.CalculateConnectionsForCellAndNeighbours (x, z);

                    }
                }

                //set the camera bounds
                CameraController.Current.SetCameraParams (worldTileDimensions, worldTileDimensions, 1);

                //create center island
                if (createCenterLand && centerLandRadius > 0) {
                    CreateCenterLand ();
                }

                hasWorld = true;

                //update all chunks
                foreach (Chunk chunk in chunks) chunk.RefreshAll ();
            }
            else {
                Debug.Log ("Cannot create world with dimensions of 0 or chunk size of zero");
            }
        }

        public void SerializeAllTiles (System.IO.BinaryWriter writer) {
            writer.Write (worldSize * CHUNK_SIZE);
            for (int z = 0 ; z < worldSize * CHUNK_SIZE ; z++) {
                for (int x = 0 ; x < worldSize * CHUNK_SIZE ; x++) {
                    TileGrid[x, z].Serialize (writer);
                }
            }
        }

        public void DeserializeWorld (System.IO.BinaryReader reader, int format) {
            int size = reader.ReadInt32 ();

            Debug.Log ($"Deserializing {size} x {size} world in format: {format}");
            CreateWorld (size / CHUNK_SIZE, createCenterLand: false);

            for (int z = 0 ; z < size ; z++) {
                for (int x = 0 ; x < size ; x++) {
                    TileGrid[x, z].Deserialize (reader, format);
                }
            }

            //update all chunks
            foreach (Chunk chunk in chunks) chunk.RefreshAll ();

            UndoHandler.ClearUndoHistory ();
        }

        public void ClearMap () {
            for (int z = 0 ; z < worldSize * CHUNK_SIZE ; z++) {
                for (int x = 0 ; x < worldSize * CHUNK_SIZE ; x++) {
                    TileGrid[x, z].ResetToDefault ();
                }
            }

            if (centerLandRadius > 0) CreateCenterLand ();
        }

        public string[] GetWorldSpecificMetaData () {
            return new string[] {
            $"size = {WorldSizeInTiles}"            };
        }

        public IEnumerable<Tile> AllTiles () {
            if (TileGrid == null) yield return null;

            for (int z = 0 ; z < worldSize * CHUNK_SIZE ; z++) {
                for (int x = 0 ; x < worldSize * CHUNK_SIZE ; x++) {
                    yield return TileGrid[x, z];
                }
            }
        }

        private void DeleteWorldWithAssets () {
            foreach (Chunk chunk in chunks) {
                Destroy (chunk.gameObject);
            }
            chunks = null;
            TileGrid = null;
            hasWorld = false;
        }


        private void CreateCenterLand () {
            var _ = (Vector2) TileGrid.Bounds;
            Vector2Int centerCoords = new Vector2Int ((int) (_.x / 2f), (int) (_.y / 2f));

            Vector2Int startCooridnates = centerCoords - new Vector2Int (centerLandRadius, centerLandRadius);
            for (int x = 0 ; x < centerLandRadius * 2 ; x++) {
                for (int z = 0 ; z < centerLandRadius * 2 ; z++) {
                    TileGrid[startCooridnates.x + x, startCooridnates.y + z].SetTerrain (TerrainType.Land);
                }
            }
        }

        private Chunk CreateChunk (int x, int z) {
            Chunk chunk = Instantiate (chunkPrefab).GetComponent<Chunk> ();
            chunk.Init (x, z);
            chunk.transform.position = new Vector3 (x, 0, z) * CHUNK_SIZE;
            return chunk;
        }

        private void OnValidate () {

            if (connectToMapEdge_lastValue == connectToMapEdge) return;
            connectToMapEdge_lastValue = connectToMapEdge;
            if (chunks == null) return;
            foreach (Chunk c in chunks) c.RefreshAll ();
        }

        private void OnDrawGizmos () {
            if (drawGizmos == false) return;
            //main grid
            Gizmos.color = Color.black;
            for (int z = 0 ; z < CHUNK_SIZE * worldSize + 1 ; z++) {
                Gizmos.DrawLine (new Vector3 (-.5f, 0, z - .5f), new Vector3 (CHUNK_SIZE * worldSize - .5f, 0, z - .5f));
            }
            for (int x = 0 ; x < CHUNK_SIZE * worldSize + 1 ; x++) {
                Gizmos.DrawLine (new Vector3 (x - .5f, 0, -.5f), new Vector3 (x - .5f, 0, CHUNK_SIZE * worldSize - .5f));
            }

            //dual grid
            Gizmos.color = Color.gray;
            for (int z = -1 ; z < CHUNK_SIZE * worldSize + 1 ; z++) {
                Gizmos.DrawLine (new Vector3 (-1, 0, z), new Vector3 (CHUNK_SIZE * worldSize, 0, z));
            }
            for (int x = -1 ; x < CHUNK_SIZE * worldSize + 1 ; x++) {
                Gizmos.DrawLine (new Vector3 (x, 0, -1), new Vector3 (x, 0, CHUNK_SIZE * worldSize));
            }

            //chunk grid
            Gizmos.color = new Color (0, 1, 0, .1f);
            for (int z = 0 ; z < worldSize ; z++) {
                for (int x = 0 ; x < worldSize ; x++) {

                    Gizmos.DrawCube (new Vector3 (z * CHUNK_SIZE + (CHUNK_SIZE / 2f) - 1f, 0, x * CHUNK_SIZE + (CHUNK_SIZE / 2f) - 1), new Vector3 (CHUNK_SIZE, 0, CHUNK_SIZE) * .95f);
                }
            }
        }
    }
}