using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Citybuilder.AI {

    public class NodeGraph {

        public static readonly Vector2Int[] offsets = new Vector2Int[] {
            new Vector2Int (0,1),
            new Vector2Int (1,1),
            new Vector2Int (1,0),
            new Vector2Int (1,-1),
            new Vector2Int (0,-1),
            new Vector2Int (-1,-1),
            new Vector2Int (-1,0),
            new Vector2Int (-1,1),
        };

        private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node> ();
        private HashSet<Node> openList;
        private HashSet<Node> closedList;
        private LinkedList<Node> fCostList;
        public NodeGraph () {
            nodes = new Dictionary<Vector2Int, Node> ();

        }

        public List<Vector2Int> FindPath (Vector2Int start, Vector2Int end, bool allowDiagonals) {

            if (nodes.ContainsKey (start) && nodes.ContainsKey (end)) {

                Node startNode = nodes[start];
                Node endNode = nodes[end];

                //use this value to clear nodes upon visiting them
                //no need to reset node we never look at
                int clearKey = UnityEngine.Random.Range (int.MinValue, int.MaxValue);

                openList = new HashSet<Node> { startNode };
                closedList = new HashSet<Node> ();
                fCostList = new LinkedList<Node> ();

                //calcualte start node g
                startNode.g = 0;
                startNode.h = GetManhattanDistance (startNode, endNode);

                //loop through while openList has values
                while (openList.Count > 0) {
                    Node currentNode = fCostList.First ();

                    //optionally clear the node
                    if (currentNode.clearKey != clearKey) ClearCosts (currentNode, clearKey);

                    //if final node finalize path
                    if (currentNode == endNode) {
                        return CalcPath (endNode);
                    }

                    openList.Remove (currentNode);
                    closedList.Add (currentNode);

                    Node[] neighbors = GetNeighbors (currentNode, allowDiagonals);

                    //cycle through all the neighbors
                    for (int i = 0 ; i < neighbors.Length ; i++) {

                        //skip nodes already on closed list
                        if (closedList.Contains (neighbors[i])) continue;

                        int tentativeCost = currentNode.g + GetManhattanDistance (currentNode, neighbors[i]);
                        if (tentativeCost < neighbors[i].g) {
                            neighbors[i].parent = currentNode;
                            neighbors[i].g = tentativeCost;

                            //calculate new h cost
                            neighbors[i].h = tentativeCost - currentNode.g;

                            //update the node in the fcost list
                            fCostList.Remove (neighbors[i].linkedListNode);
                            AddNodeToFCostList (neighbors[i]);

                            //add neighbor to open list
                            if (!openList.Contains (neighbors[i])) {
                                openList.Add (neighbors[i]);
                            }
                        }
                    }
                }

                //out of node on open list and no path findable
                return null;


            }
            else {
                //start and/or end nodes are missing, return an empty path
                return new List<Vector2Int> (0);
            }
        }
        private Node[] GetNeighbors (Node node, bool allowDiagonals) {
            if (allowDiagonals) {
                Node[] neighbors = new Node[8];
                for (int i = 0 ; i < offsets.Length ; i++) {
                    Vector2Int key = new Vector2Int (node.x, node.z) + offsets[i];

                    if (nodes.ContainsKey (key)) {
                        neighbors[i] = nodes[key];
                    }
                }
                return neighbors;
            }
            else {
                Node[] neighbors = new Node[4];
                for (int j = 0 ; j < offsets.Length ; j += 2) {
                    Vector2Int key = new Vector2Int (node.x, node.z) + offsets[j];

                    if (nodes.ContainsKey (key)) {
                        neighbors[j / 2] = nodes[key];
                    }
                }
                return neighbors;
            }
        }

        private List<Vector2Int> CalcPath (Node node) {
            List<Vector2Int> points = new List<Vector2Int>();
            while (node.parent != null) {
                points.Add (new Vector2Int (node.x, node.z));
                node = node.parent;
            }
            points.Reverse ();
            return points;
        }

        private void ClearCosts (Node node, int clearKey) {
            node.clearKey = clearKey;
            node.g = int.MaxValue;
            node.parent = null;
            node.linkedListNode = null;
        }

        private int GetManhattanDistance (Node a, Node b) {
            return Mathf.Abs (a.x - b.x) + Mathf.Abs (a.z - b.z);
        }

        private void AddNodeToFCostList (Node node) {

            if (fCostList.Count == 0) {
                fCostList.AddFirst (node);
                return;
            }

            //search the list for nodes with sorting
            int f = node.f;
            LinkedListNode<Node> previousNode = fCostList.First;

            //search through every node to f
            for (int i = 0 ; i < fCostList.Count ; i++) {
                LinkedListNode<Node> current = previousNode.Next;

                //if the cvurrent node is too small search next node
                if (current.Value.f < f) {
                    previousNode = current;
                    continue;
                }

                //if the current value is the same, add the node after
                if (current.Value.f == f) {
                    current.Value.linkedListNode = fCostList.AddAfter (current, node);
                }

                //if the current value is bigger, add before the previous node
                if (current.Value.f > f) {
                    current.Value.linkedListNode = fCostList.AddBefore (current, node);
                }
            }

        }
    }
}
