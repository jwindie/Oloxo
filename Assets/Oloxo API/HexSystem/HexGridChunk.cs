using UnityEngine;
using UnityEngine.UI;

namespace Oloxo.HexSystem {

    public class HexGridChunk : MonoBehaviour {

        HexCell[] cells;

        HexMesh hexMesh;

        public void Awake ()  {
            hexMesh = GetComponentInChildren<HexMesh> ();

            cells = new HexCell[HexMetrics.CHUNK_SIZE_X * HexMetrics.CHUNK_SIZE_Z];
        }

        /// <summary>
        /// Adds a cell into the chunk.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="cell"></param>
        public void AddCell (int index, HexCell cell) {
            cells[index] = cell;
            cell.chunk = this;
            cell.transform.SetParent (transform, false);
        }

        public void Refresh () {
            enabled = true;
        }

        //use late update toe refresh the chunk and then disable itself
        private void LateUpdate () {
            hexMesh.Triangulate (cells);
            enabled = false;
        }
    }
}