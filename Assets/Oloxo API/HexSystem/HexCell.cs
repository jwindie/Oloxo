using UnityEngine;

namespace Oloxo.HexSystem {

    public class HexCell : MonoBehaviour {
        //Cells do not need to be anything but data and do not need to inherit from
        //MonoBehaviour. 

        public HexCoordinates coordinates;
        public bool harvested;

        [SerializeField] HexCell[] neighbors;

        public HexCell() {
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

        private void OnDrawGizmosSelected () {
            //Draw a red dot at the position of all neighbor tiles
            Gizmos.color = Color.red;
            Gizmos.DrawSphere (transform.position, 1f);

            for (int i = 0 ; i < neighbors.Length ; i++) {
                if (neighbors[i] != null) {
                    Gizmos.DrawSphere (neighbors[i].transform.position, .7f);
                    Gizmos.DrawLine(transform.position, neighbors[i].transform.position);
                }
            }
        }
    }
}