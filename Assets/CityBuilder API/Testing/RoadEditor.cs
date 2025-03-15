using Citybuilder.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Citybuilder.Testing {
    public static class RoadEditor {

        private enum DragDirection {
            None,
            X,
            Z
        }

        private static bool draggingRoad;
        private static Vector2Int startPoint;
        private static Vector2Int endPoint;
        private static Vector2Int constrainedEndPoint;
        private static int roadElevation;
        private static DragDirection dragDirection;



        public static Vector3[] GetDebugPath {
            get {
                return new Vector3[] {
                    new Vector3 (startPoint.x, roadElevation, startPoint.y),
                    new Vector3 (constrainedEndPoint.x, roadElevation, constrainedEndPoint.y),
                };
            }
        }
        public static void HandleTouch (Vector2Int tileCoordinates) {


            if (Input.GetMouseButtonDown (0)) {
                BeginDrag (tileCoordinates);
            }
            else if (Input.GetMouseButtonUp (0)) {
                EndDrag ();
            }

            if (draggingRoad) {
                //check to see if the endpoint has moved
                if (tileCoordinates != endPoint) {
                    endPoint = tileCoordinates;

                    //figure out the drag direction
                    if (endPoint == startPoint) dragDirection = DragDirection.None; //no direction
                    else {
                        if (dragDirection == 0) {
                            var dragDirectionVector = startPoint - endPoint;
                            if (dragDirectionVector.x != 0) dragDirection = DragDirection.X; //x first
                            else if (dragDirectionVector.y != 0) dragDirection = DragDirection.Z; //z first

                            Debug.Log ($"DragDirection: {dragDirection}");
                        }

                        //only calculate road if drag has a direction 
                        ConstrainEndpoint ();
                    }
                }
            }
        }

        private static void BeginDrag (Vector2Int coords) {
            draggingRoad = true;
            startPoint = constrainedEndPoint = endPoint = coords;
            Debug.Log ("Starting Road Drag");
        }

        private static void EndDrag () {
            draggingRoad = false;
        }

        /// <summary>
        /// Attempts to calculate a manhattan connection between the start and end points
        /// </summary>
        private static void ConstrainEndpoint () {
            constrainedEndPoint = endPoint;

            //can the road be reacehd with a single line?
            //if so modify the drag direction
            Vector2Int diff = new Vector2Int (
                Mathf.Abs (startPoint.x - endPoint.x),
                Mathf.Abs (startPoint.y - endPoint.y)
                );
            Debug.Log ($"Diff: { diff}");

            if (diff != Vector2Int.zero) {

                if (diff.x > diff.y) {
                    dragDirection = DragDirection.X;
                    constrainedEndPoint.y = startPoint.y;
                }
                else {
                    dragDirection = DragDirection.Z;
                    constrainedEndPoint.x = startPoint.x;
                }
            }
        }

        private static void PlaceRoads (Vector2Int start, Vector2Int direction, int distance) {
            Debug.Log ($"ROAD DISTANCE: {distance}");
            for (int i = 0 ; i <= distance ; i++) {
                World.Current.TileGrid.GetValue (start + (direction * i)).SetRoad (Roads.RoadType.Basic);
                Debug.Log (start + (direction * distance));
            }
        }


        /// <summary>
        /// Returns true if the two points can be connected by a straight orthographic line
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool InLineWithOrtho (this Vector2Int a, Vector2Int b) {
            return a.x == b.x || a.y == b.y;
        }
    }
}