using System.Collections.Generic;
using UnityEngine;

namespace Oloxo.HexSystem {

    public class HexCell : MonoBehaviour {
        //Cells do not need to be anything but data and do not need to inherit from
        //MonoBehaviour. 

        public HexCoordinates coordinates;
        public HexGridChunk chunk;

        [SerializeField] HexCell[] neighbors;

        bool harvested;

        Terrain terrain = default;

        //river data
        List<HexDirection> riverDirections = new List<HexDirection> (6);

        public bool Harvested {
            get { return harvested; }
            set {
                if (value != harvested) {
                    harvested = value;
                    Refresh ();
                }
            }
        }

        public Terrain Terrain {
            get { return terrain; }
            set {
                if (terrain != value) {
                    terrain = value;
                    Refresh ();
                }
            }
        }

        /// <summary>
        /// Is there a river running through this tile?
        /// </summary>
        public bool HasRiver {
            get {
                return riverDirections.Count > 0;
            }
        }


        /// <summary>
        /// Is the river the start or the end node of a river?
        /// </summary>
        public bool HasRiverStartOrEnd {
            get {
                return riverDirections.Count == 1;
            }
        }

        public bool HasRiverThroughEdge (HexDirection direction) {
            //iterate over all the valid river connections and return true if any are true;
            for (int i = 0 ; i < riverDirections.Count ; i++) if (riverDirections[i] == direction) return true;
            return false;
        }

        /// <summary>
        /// Returns the river index assigned to this direction.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public int GetRiverIndex (HexDirection dir) {
            for (int i = 0 ; i < riverDirections.Count ; i++) if (riverDirections[i] == dir) return i;
            return -1;
        }

        /// <summary>
        /// Removes the river that connects in the direction provided.
        /// <para>
        /// If the provided direction is E, then the river connecting towards E is removed.
        /// </para>
        /// </summary>
        /// <param name="direction"></param>
        public void RemoveRiver (HexDirection direction) {
            RemoveRiver (GetRiverIndex (direction));
        }

        /// <summary>
        /// Removes all rivers on the tile.
        /// </summary>
        public void RemoveAllRivers () {
            for (int i = 0 ; i < riverDirections.Count ; i++) {
                RemoveRiver (i);
            }
        }

        public void RemoveRiver (int index) {
            if (index >= riverDirections.Count) {
                return;
            }


            //remove the rive on the neighbor tile
            HexCell neighbor = GetNeighbor (riverDirections[index]);
            neighbor.riverDirections.Remove (riverDirections[index].Opposite ());
            neighbor.RefreshSelfOnly ();

            //remove the river connection on this tile
            riverDirections.RemoveAt (index);
            RefreshSelfOnly ();
        }

        /// <summary>
        /// Removes all incoming and outgoing rivers on the tile.
        /// </summary>
        public void RemoveRiver () {
            RemoveAllRivers ();
        }

        public HexCell () {
            neighbors = new HexCell[6];
        }

        public HexCell GetNeighbor (HexDirection direction) {
            return neighbors[(int) direction];
        }

        public void SetNeighbor (HexDirection direction, HexCell cell) {
            neighbors[(int) direction] = cell;

            //when setting one neighbor, set the other as well
            cell.neighbors[(int) direction.Opposite ()] = this;
        }

        void Refresh () {
            if (chunk) {
                chunk.Refresh ();
                RefreshNeighbors ();
            }
        }

        /// <summary>
        /// Does not refresh the cell's neighbors.
        /// </summary>
        void RefreshSelfOnly () {
            if (chunk) {
                chunk.Refresh ();
            }
        }

        void RefreshNeighbors () {
            for (int i = 0 ; i < neighbors.Length ; i++) {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk) {
                    neighbor.chunk.Refresh ();
                }
            }
        }

        private void OnDrawGizmos() {
            //Draw a red dot at the position of all neighbor tiles
            Gizmos.color = Color.red;
            Gizmos.DrawSphere (transform.position, 1f);

            for (int i = 0 ; i < 3; i++) {
                if (neighbors[i] != null) {
                    if (riverDirections.Contains ((HexDirection) i)){
                        Gizmos.color = Color.blue;
                    }
                    else {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawLine (transform.position, neighbors[i].transform.position);
                }
            }
        }
    }
}