using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Zengardens.AI {
    public class Pathfinding {

        private const int COST_STRAIGHT = 10;
        private const int COST_DIAGONAL = 60;

        private Dictionary<Vector2Int, PathNode> nodeDictionary;

        private List<PathNode> openList;
        private List<PathNode> closedList;

        public Pathfinding (int width, int height, int cellSize) {

            //new grid
            nodeDictionary = new Dictionary<Vector2Int, PathNode> ();
        }

        public PathNode[] Nodes {
            get {
                return nodeDictionary.Values.ToArray ();
            }
        }

        public PathNode GetNode (int x, int y) {
            return GetNode (new Vector2Int (x, y));
        }

        public PathNode GetNode (Vector2Int coords) {
            if (nodeDictionary.TryGetValue (coords, out PathNode node)) {
                return node;
            }
            return null;
        }


        public void AddNode (PathNode node) {
            if (node == null) {
                Debug.Log ("NODE IS NULL ON ADD");
                return;
            }
            if (!nodeDictionary.ContainsKey (node.Coordinates)) {

                nodeDictionary.Add (node.Coordinates, node);

                if (nodeDictionary.TryGetValue (node.Coordinates, out PathNode test)) {
                    if (test == null) {
                        Debug.Log ("NODE IS NULL AFTER ADD");
                    }
                    else {
                        //Debug.Log ("NODE ADDED PERFECTLY");
                    }
                }
            }
            else {
                //Debug.Log ($"NODE ALREADDY EXISTS AT THESE COORDINATES: {node.Coordinates}");
            }
        }

        //public List<AIPathNode> FindPath (Vector2Int startCoords, Vector2Int endCoords) {
        //    AIPathNode startNode = nodeGrid.GetValue (startCoords);
        //    AIPathNode endNode = nodeGrid.GetValue (endCoords);

        //    openList = new List<AIPathNode> { startNode };
        //    closedList = new List<AIPathNode> ();

        //    for (int x = 0 ; x < nodeGrid.Width ; x++) {
        //        for (int y = 0 ; y < nodeGrid.Height ; y++) {
        //            AIPathNode node = nodeGrid.GetValue (x, y);
        //            node.gCost = int.MaxValue;
        //            node.CalculateFCost ();
        //            node.fromNode = null;
        //        }
        //    }

        //    //calculate start node costs
        //    startNode.gCost = 0;
        //    startNode.hCost = CalculateDistanceCost (startNode, endNode);
        //    startNode.CalculateFCost ();

        //    //start loop for AI navigation
        //    //for now, put in a coroutine or something?
        //    //nah fuck it
        //    //just let it run ablaze
        //    while (openList.Count > 0) {
        //        Debug.Log ("Searching...");
        //        PathNode currentNode = GetLowestFCostNode (openList);
        //        if (currentNode.Equals (endNode)) {
        //            //reached final node
        //            Debug.Log ("Done!");
        //            return CalculatePath (endNode);
        //        }

        //        //if not the end node
        //        openList.Remove (currentNode);
        //        closedList.Add (currentNode);

        //        foreach (PathNode neighbourNode in GetNeighbourList (currentNode)) {
        //            //if the neighbor hass been searched, skip it
        //            if (closedList.Contains (neighbourNode) ||
        //                !WorldManager_OLD.Current.GetTileEdge (new Vector2Int (currentNode.x, currentNode.y), new Vector2Int (neighbourNode.x, neighbourNode.y)).open
        //                ) continue;

        //            //if the node is not walkable, add t closed list and continue
        //            if (!neighbourNode.walkable) {
        //                closedList.Add (neighbourNode);
        //                continue;
        //            }

        //            int tentativeGCost = currentNode.gCost + CalculateDistanceCost (currentNode, neighbourNode);

        //            //directional change cost//1 if forward, -1 if right
        //            if (currentNode.fromNode != null) {
        //                int currentDirectionalAxis = (currentNode.x == currentNode.fromNode.x) ? 1 : 0;
        //                //is the neighbor node in a different direction? if so add cost
        //                if (currentDirectionalAxis == 1 && neighbourNode.x != currentNode.x) tentativeGCost += 100;
        //                if (currentDirectionalAxis == 0 && neighbourNode.y != currentNode.y) tentativeGCost += 100;
        //            }

        //            //check if we have a faster path from the current node than we had previously
        //            if (tentativeGCost < neighbourNode.gCost) {

        //                //if we do have a better paath, update it
        //                neighbourNode.fromNode = currentNode;
        //                neighbourNode.gCost = tentativeGCost;
        //                neighbourNode.hCost = CalculateDistanceCost (neighbourNode, endNode);
        //                neighbourNode.CalculateFCost ();

        //                //make sure the neighbor is in the open list
        //                if (!openList.Contains (neighbourNode)) openList.Add (neighbourNode);
        //            }
        //        }
        //    }
        //    Debug.Log ("Failed!");

        //    //out of nodes on the open list
        //    //we have searched the whole map and found no path
        //    MostRecentPath = new PathNode[0];
        //    return null;
        //}

        //public int CalculateDistanceCost (PathNode a, PathNode b) {
        //    int xDist = Mathf.Abs (a.x - b.x);
        //    int yDist = Mathf.Abs (a.y - b.y);
        //    int remain = Mathf.Abs (xDist - yDist);
        //    return COST_DIAGONAL * Mathf.Min (xDist, yDist) + COST_STRAIGHT * remain;
        //}

        //private PathNode GetLowestFCostNode (List<PathNode> pathNodeList) {
        //    PathNode lowestFCostNode = pathNodeList[0];
        //    for (int i = 0 ; i < pathNodeList.Count ; i++) {
        //        if (pathNodeList[i].fCost < lowestFCostNode.fCost) lowestFCostNode = pathNodeList[i];
        //    }
        //    return lowestFCostNode;
        //}

        //private List<PathNode> CalculatePath (PathNode endNode) {
        //    List<PathNode> path = new List<PathNode> ();
        //    path.Add (endNode);
        //    PathNode currentNode = endNode;

        //    //loop through each nodes from node until there are no more
        //    //reverse the path and viola!
        //    while (currentNode.fromNode != null) {
        //        path.Add (currentNode.fromNode);
        //        currentNode = currentNode.fromNode;
        //    }
        //    path.Reverse ();
        //    MostRecentPath = path.ToArray ();
        //    return path;
        //}

        //private List<PathNode> GetNeighbourList (PathNode currentNode) {
        //    List<PathNode> neighbourList = new List<PathNode> (4);
        //    //left
        //    if (currentNode.x - 1 >= 0) {
        //        neighbourList.Add (nodeGrid.GetValue (currentNode.x - 1, currentNode.y));
        //    }
        //    //right
        //    if (currentNode.x + 1 < nodeGrid.Width) {
        //        neighbourList.Add (nodeGrid.GetValue (currentNode.x + 1, currentNode.y));
        //    }
        //    //up
        //    if (currentNode.y - 1 >= 0) neighbourList.Add (nodeGrid.GetValue (currentNode.x, currentNode.y - 1));
        //    //down
        //    if (currentNode.y + 1 < nodeGrid.Height) neighbourList.Add (nodeGrid.GetValue (currentNode.x, currentNode.y + 1));

        //    return neighbourList;
        //}

        //private Vector3 GetTileAtPathNode (PathNode node) {
        //    if (WorldManager_OLD.Current != null) {
        //        return WorldManager_OLD.Current.GetTopAnchorPoint (node.x, node.y);
        //    }
        //    else return Vector3.one * float.NaN;
        //}
    }
}
