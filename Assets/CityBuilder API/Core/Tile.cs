using Citybuilder.Roads;
using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Core {

    [System.Serializable]
    public class Tile {

        public static readonly Vector3[] CornerOffsets = new Vector3[]{
            new Vector3(-.5f,0, .5f),
            new Vector3(-.5f,0, .5f),
            new Vector3(.5f,0, -.5f),
            new Vector3(-.5f,0, -.5f)
        };

        public TerrainType TerrainType { get; private set; }
        public int x { get; private set; }
        public int z { get; private set; }

        public Chunk Chunk {
            get {
                return chunks[0];
            }
        }
        public Vector3 WorldPosition { get; private set; }
        public Color Color { get; private set; }

        private List<Chunk> chunks;
        public bool HollowState { get; private set; }
        public RoadType RoadType { get; private set; }
        public int TreeState { get; private set; }
        public bool HarborState { get; private set; }
        public float RandomIndexedRotation { get; private set; }

        public Tile (int x, int z, Vector3 worldPosition, List<Chunk> chunks, TerrainType terrain = TerrainType.Water) {
            this.x = x;
            this.z = z;
            this.WorldPosition = worldPosition;
            Color = Color.white;
            TerrainType = terrain;
            this.chunks = chunks;
            RandomIndexedRotation = Random.Range (0, 4);
        }

        private void RefreshAll () {
            foreach (Chunk _ in chunks) _.RefreshAll ();
        }

        public void SetTerrain (TerrainType value) {
            if (value == this.TerrainType) return;

            //record tile state to undo
            var _a = TerrainType;
            UndoHandler.RegisterUndo (() => SetTerrain (_a));


            this.TerrainType = value;
            if (value == TerrainType.Water) {
                //save tree value if there are trees
                if (TreeState > 0) {
                    var _t = TreeState;
                    UndoHandler.RegisterUndo (() => SetTree (_t));
                    TreeState = 0;
                }
            }

            SetAIGraphTag ();

            foreach (Chunk _ in chunks) _.RefreshTerrain ();

        }

        public void SetRoad (RoadType value) {
            if (value == RoadType) return;
            if (TerrainType == TerrainType.Water && !HarborState) return;

            //record tile state to undo
            var _a = RoadType;
            var _b = HollowState;
            var _c = TreeState;
            UndoHandler.RegisterUndo (() => SetRoad (_a));
            UndoHandler.RegisterUndo (() => SetHollow (_b));
            UndoHandler.RegisterUndo (() => SetTree (_c));

            RoadType = value;
            HollowState = value > RoadType.None;
            TreeState = 0;
            foreach (Chunk _ in chunks) {
                _.RefreshRoads ();
                _.RefreshClip ();
            }
        }

        public void SetTree (int value) {
            if (TreeState == value
                || TerrainType == TerrainType.Water
                || RoadType > RoadType.None
                ) return; //only set if tile is land

            //record tile state to undo
            var _a = TreeState;
            UndoHandler.RegisterUndo (() => SetTree (_a));

            TreeState = value;

            //trees do not have connected Edges with other chunks and can refresh in isolation
            chunks[0].RefreshFoliage ();
        }

        public void SetHollow (bool state) {
            if (HollowState == state || TerrainType != TerrainType.Land) return;

            //record tile state to undo
            var _a = HollowState; ;
            UndoHandler.RegisterUndo (() => SetHollow (_a));

            HollowState = state;

            //clip does not have connected Edges with other chunks and can refresh in isolation
            chunks[0].RefreshClip ();
        }

        public void SetHarbor (bool state) {
            if (HarborState == state || TerrainType != TerrainType.Water) return;

            //record tile state to undo
            var _a = HarborState; ;
            UndoHandler.RegisterUndo (() => SetHarbor (_a));

            HarborState = state;

            //harbors do not have connected Edges with other chunks and can refresh in isolation
            chunks[0].RefreshTerrain ();
        }

        public void Serialize (System.IO.BinaryWriter writer) {
            writer.Write (Color.r);
            writer.Write (Color.g);
            writer.Write (Color.b);

            writer.Write ((int) TerrainType);
            writer.Write (HollowState);
            writer.Write (TreeState);
            writer.Write ((int) RoadType);
            writer.Write (HarborState);
        }

        public void Deserialize (System.IO.BinaryReader reader, int format) {

            switch (format) {

                case 1:

                    Color = new Color (
                        reader.ReadSingle (),
                        reader.ReadSingle (),
                        reader.ReadSingle (),
                        1);
                    TerrainType = (TerrainType) reader.ReadInt32 ();
                    HollowState = reader.ReadBoolean ();
                    TreeState = reader.ReadInt32 ();
                    RoadType = (RoadType) reader.ReadInt32 ();
                    HarborState = reader.ReadBoolean ();
                    break;
            }

            SetAIGraphTag ();
        }

        public void ResetToDefault () {
            Color = Color.white;
            TerrainType = TerrainType.Water;
            HollowState = false;
            TreeState = 0;
            RoadType = RoadType.None;
            HarborState = false;
            RefreshAll ();
        }

        private void SetAIGraphTag () {
            //logic for setting the node on the grid graph
            World.Current.AstarPath.AddWorkItem (() => {
                var gg = World.Current.AstarPath.data.gridGraph;
                var node = gg.GetNode (x, z);
                // This example uses perlin noise to generate the map
                node.Walkable = TerrainType == TerrainType.Land;
                gg.CalculateConnectionsForCellAndNeighbours (x, z);
                //Debug.Log ($"{value}/{node.Walkable}");
            });
        }
    }
}