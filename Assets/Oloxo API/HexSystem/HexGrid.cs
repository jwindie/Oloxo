using Oloxo.Core;
using System;
using UnityEngine;

namespace Oloxo.HexSystem {

    public class HexGrid : MonoBehaviour {

        public int width, height;
        public HexCell cellPrefab;

        private HexCell[] cells;
        private HexMesh hexMesh;

        /// <summary>
        /// Functions as the awake function but is called manually.
        /// </summary>
        /// <returns></returns>
        public HexGrid Init() {

            //get and init the hex mesh
            hexMesh = GetComponentInChildren<HexMesh> ().Init ();

            //create the array of cells and populate it
            cells = new HexCell[width * height];
            App.Current.LogHandler.StartBlock ($"[INIT] Created hex grid {width}x{height} ({width * height}) cells");
            
            for (int z = 0, i = 0 ; z < height ; z++) {
                for (int x = 0 ; x < width ; x++) {
                    CreateCell (x, z, i++);
                }
            }

            return this;
        }

        public HexCell GetCell (Vector3 position) {
            position = transform.InverseTransformPoint (position);
            HexCoordinates coordinates = HexCoordinates.FromPosition (position);

            //get the index of the cell (may be out of bounds)
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            HexCell cell = cells[index];


            return cells[index];
        }

        public void Refresh () { 
            hexMesh.Triangulate (cells);
        }

        private void Start () {
            hexMesh.Triangulate (cells);
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
            cell.transform.SetParent (transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates (x, z);

            //set the neighbors on the cell
            if (x > 0) {//E -> W connection
                cell.SetNeighbor (HexDirection.W, cells[i - 1]);
            }
            if (z > 0) {
                if ((z & 1) == 0) {
                    cell.SetNeighbor (HexDirection.SE, cells[i - width]);
                    if (x > 0) {
                        cell.SetNeighbor (HexDirection.SW, cells[i - width - 1]);
                    }
                }
                else {
                    cell.SetNeighbor (HexDirection.SW, cells[i - width]);
                    if (x < width - 1) {
                        cell.SetNeighbor (HexDirection.SE, cells[i - width + 1]);
                    }
                }
            }
        }
    }
}