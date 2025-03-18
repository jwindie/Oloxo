using Oloxo.Core;
using System;
using UnityEngine;

namespace Oloxo.HexSystem {

    public class HexGrid : MonoBehaviour {

        public int chunkCountX = 4, chunkCountZ = 3;
        public HexCell cellPrefab;
        public HexGridChunk chunkPrefab;

        private HexCell[] cells;
        private HexGridChunk[] chunks;
        private int cellCountX, cellCountZ;

        private Vector2 cameraBounds = new Vector2 ();

        /// <summary>
        /// Functions as the awake function but is called manually.
        /// </summary>
        /// <returns></returns>
        public HexGrid Init () {

            //get and init the hex mesh

            cellCountX = chunkCountX * HexMetrics.CHUNK_SIZE_X;
            cellCountZ = chunkCountZ * HexMetrics.CHUNK_SIZE_Z;

            CreateChunks ();
            CreateCells ();

            //update the bounds of the camera. The last cell will be the farthest away
            App.Current.Game.CameraController.SetCameraBounds (
                -Vector3.one * HexMetrics.OUTER_RADIUS,
                cells[cells.Length - 1].transform.position + Vector3.one * HexMetrics.OUTER_RADIUS
            ).CenterInBounds ();

            return this;
        }


        public HexCell GetCell (Vector3 position) {
            position = transform.InverseTransformPoint (position);
            HexCoordinates coordinates = HexCoordinates.FromPosition (position);

            //get the index of the cell (may be out of bounds)
            int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
            HexCell cell = cells[index];


            return cells[index];
        }

        public HexCell GetCell (HexCoordinates coordinates) {
            int z = coordinates.Z;
            if (z < 0 || z >= cellCountZ) {
                return null;
            }
            int x = coordinates.X + z / 2;
            if (x < 0 || x >= cellCountX) {
                return null;
            }
            return cells[x + z * cellCountX];
        }

        void CreateChunks () {
            //creates an array of chunks
            chunks = new HexGridChunk[chunkCountX * chunkCountZ];

            //create chunk objetcs and put them into the array
            for (int z = 0, i = 0 ; z < chunkCountZ ; z++) {
                for (int x = 0 ; x < chunkCountX ; x++) {
                    HexGridChunk chunk = chunks[i++] = Instantiate (chunkPrefab);
                    chunk.transform.SetParent (transform);
                }
            }
        }

        private void CreateCells () {

            //create the array of cells and populate it
            cells = new HexCell[cellCountX * cellCountZ];
            App.Current.LogHandler.StartBlock ($"[INIT] Created hex grid {cellCountX}x{cellCountZ} ({cellCountX * cellCountZ}) cells");

            for (int z = 0, i = 0 ; z < cellCountZ ; z++) {
                for (int x = 0 ; x < cellCountX ; x++) {
                    CreateCell (x, z, i++);
                }
            }
        }

        /// <summary>
        /// Creates a single cell.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="i"></param>
        void CreateCell (int x, int z, int i) {

            Vector3 position;
            //set the position with offset corrdinates
            //offset the rows by half x each new row, but then shift every other
            //to make them appear in a hexagonal alignment
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.INNER_RADIUS * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.OUTER_RADIUS * 1.5f);

            //create cell, parent and position it, and set its coords
            HexCell cell = cells[i] = Instantiate<HexCell> (cellPrefab);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates (x, z);

            //set the neighbors on the cell
            if (x > 0) {//E -> W connection
                cell.SetNeighbor (HexDirection.W, cells[i - 1]);
            }
            if (z > 0) {
                if ((z & 1) == 0) {
                    cell.SetNeighbor (HexDirection.SE, cells[i - cellCountX]);
                    if (x > 0) {
                        cell.SetNeighbor (HexDirection.SW, cells[i - cellCountX - 1]);
                    }
                }
                else {
                    cell.SetNeighbor (HexDirection.SW, cells[i - cellCountX]);
                    if (x < cellCountX - 1) {
                        cell.SetNeighbor (HexDirection.SE, cells[i - cellCountX + 1]);
                    }
                }
            }

            AddCellToChunk (x, z, cell);
        }

        /// <summary>
        /// Places a cell into a chunk.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="cell"></param>
        void AddCellToChunk (int x, int z, HexCell cell) {
            //find the correct chunk with iteger division
            int chunkX = x / HexMetrics.CHUNK_SIZE_X;
            int chunkZ = z / HexMetrics.CHUNK_SIZE_Z;
            HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

            //determine the cells local index within its chunk
            int localX = x - chunkX * HexMetrics.CHUNK_SIZE_X;
            int localZ = z - chunkZ * HexMetrics.CHUNK_SIZE_Z;

            //add to chunk
            chunk.AddCell (localX + localZ * HexMetrics.CHUNK_SIZE_X, cell);
        }
    }
}