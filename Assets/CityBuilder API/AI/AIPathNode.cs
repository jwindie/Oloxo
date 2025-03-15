using UnityEngine;

namespace Zengardens.AI {
    public class PathNode {
        public int x { get; private set; }
        public int y { get; private set; }

        public int gCost;
        public int hCost;
        public int fCost;

        public PathNode fromNode;
        public bool walkable = true;

        public PathNode[] neighbors;

        public Vector2Int Coordinates {
            get {
                return new Vector2Int (x, y);
            }
        }

        public PathNode (int x, int y) {
            this.x = x;
            this.y = y;
            neighbors = new PathNode[8];
        }

        public PathNode (Vector2Int coords) {
            x = coords.x;
            y = coords.y;
            neighbors = new PathNode[8];
        }

        public void SetNeighbor (PathNode node, int direction) {
            if (direction < 0 || direction > 7) return;
            neighbors[direction] = node;
        }


        public void CalculateFCost () {
            fCost = gCost + hCost;
        }
    }
}