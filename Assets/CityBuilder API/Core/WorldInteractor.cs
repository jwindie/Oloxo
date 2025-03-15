using Citybuilder.Testing;
using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Core {

    public static class WorldInteractor {

        public static void HandleTouch (Vector3 touchPoint) {

            Vector2Int coords = World.Current.TileGrid.GetXY (touchPoint + new Vector3 (.5f, 0, .5f));
            Tile tile = World.Current.TileGrid.GetValue (coords);
            if (tile != null) {


                Selector.Current.SetVisibility (true);
                Selector.Current.SetPosition (tile.WorldPosition);

                if (Input.GetMouseButton (0)) {
                    //terrain settings and default interaction mode
                    if (MapEditor.Current.terrainSetting.enabled) {
                        SetTerrainForArea (Selector.Current.CoordinatePosition, Selector.Current.Size, MapEditor.Current.terrainSetting.state ? TerrainType.Land : TerrainType.Water);
                        //tile.SetTerrain (MapEditor.Current.terrainSetting.state ? TerrainType.Land : TerrainType.Water);
                    }
                    if (MapEditor.Current.treeSetting.enabled) {
                        SetTreeForArea (Selector.Current.CoordinatePosition, Selector.Current.Size, MapEditor.Current.treeSetting.state ? 1 : 0);
                        //tile.SetTree (MapEditor.Current.treeSetting.state ? 1 : 0);
                    }
                    if (MapEditor.Current.roadSetting.enabled) {
                        SetRoadForArea (Selector.Current.CoordinatePosition, Selector.Current.Size, MapEditor.Current.roadSetting.state ? Roads.RoadType.Basic : Roads.RoadType.None);
                        //tile.SetRoad (MapEditor.Current.roadSetting.state ? Roads.RoadType.Basic : Roads.RoadType.None);
                    }
                    if (MapEditor.Current.hollowSetting.enabled) {
                        SetHollowForArea (Selector.Current.CoordinatePosition, Selector.Current.Size, MapEditor.Current.hollowSetting.state );
                        //tile.SetHollow (MapEditor.Current.hollowSetting.state);
                    }

                    if (MapEditor.Current.harborSetting.enabled) {
                        SetHarborForArea (Selector.Current.CoordinatePosition, Selector.Current.Size, MapEditor.Current.harborSetting.state);
                        //tile.SetHollow (MapEditor.Current.hollowSetting.state);
                    }

                    //do a little pathfinding test from one point to another
                    GameManager.Current.target.position = tile.WorldPosition;
                }
            }
        }

        private static void SetTerrainForArea (Vector2Int startTile, Vector2Int size, TerrainType terrainType) {

            //Debug.Log ($"{startTile} {size}");
            int xDirection = size.x < 0 ? -1 : 1;
            int zDirection = size.y < 0 ? -1 : 1;
            //Debug.Log ($"{xDirection} / {zDirection}");
            //Debug.Log ("Setting");

            for (int z = startTile.y ; z != startTile.y - size.y ; z -= zDirection) {
                if (z < 0 || z >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;

                //Debug.Log ("Z");
                for (int x = startTile.x ; x != startTile.x + size.x ; x += xDirection) {
                    if (x < 0 || x >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;
                    //Debug.Log ("X");

                    World.Current.TileGrid[x, z].SetTerrain (terrainType);
                    //Debug.Log ("B");

                }
            }
        }

        private static void SetTreeForArea (Vector2Int startTile, Vector2Int size, int treeState) {
            int xDirection = size.x < 0 ? -1 : 1;
            int zDirection = size.y < 0 ? -1 : 1;
            //Debug.Log ($"{xDirection} / {zDirection}");
            //Debug.Log ("Setting");

            for (int z = startTile.y ; z != startTile.y - size.y ; z -= zDirection) {
                if (z < 0 || z >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;

                //Debug.Log ("Z");
                for (int x = startTile.x ; x != startTile.x + size.x ; x += xDirection) {
                    if (x < 0 || x >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;
                    //Debug.Log ("X");
                    World.Current.TileGrid[x, z].SetTree (treeState);
                }
            }
        }

        private static void SetRoadForArea (Vector2Int startTile, Vector2Int size, Roads.RoadType roadType) {
            int xDirection = size.x < 0 ? -1 : 1;
            int zDirection = size.y < 0 ? -1 : 1;
            //Debug.Log ($"{xDirection} / {zDirection}");
            //Debug.Log ("Setting");

            for (int z = startTile.y ; z != startTile.y - size.y ; z -= zDirection) {
                if (z < 0 || z >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;

                //Debug.Log ("Z");
                for (int x = startTile.x ; x != startTile.x + size.x ; x += xDirection) {
                    if (x < 0 || x >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;
                    //Debug.Log ("X");
                    World.Current.TileGrid[x, z].SetRoad (roadType);
                }
            }
        }

        private static void SetHollowForArea (Vector2Int startTile, Vector2Int size, bool isHollow) {
            int xDirection = size.x < 0 ? -1 : 1;
            int zDirection = size.y < 0 ? -1 : 1;
            //Debug.Log ($"{xDirection} / {zDirection}");
            //Debug.Log ("Setting");

            for (int z = startTile.y ; z != startTile.y - size.y ; z -= zDirection) {
                if (z < 0 || z >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;

                //Debug.Log ("Z");
                for (int x = startTile.x ; x != startTile.x + size.x ; x += xDirection) {
                    if (x < 0 || x >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;
                    //Debug.Log ("X");
                    World.Current.TileGrid[x, z].SetHollow (isHollow);
                }
            }
        }

        private static void SetHarborForArea (Vector2Int startTile, Vector2Int size, bool isHarbor) {
            int xDirection = size.x < 0 ? -1 : 1;
            int zDirection = size.y < 0 ? -1 : 1;
            //Debug.Log ($"{xDirection} / {zDirection}");
            //Debug.Log ("Setting");

            for (int z = startTile.y ; z != startTile.y - size.y ; z -= zDirection) {
                if (z < 0 || z >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;

                //Debug.Log ("Z");
                for (int x = startTile.x ; x != startTile.x + size.x ; x += xDirection) {
                    if (x < 0 || x >= World.Current.WorldSizeInChunks * World.Current.ChunkSize) continue;
                    //Debug.Log ("X");
                    World.Current.TileGrid[x, z].SetHarbor (isHarbor);
                }
            }
        }
    }
}
