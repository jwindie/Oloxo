//Author: Jordan Williams 
//Date:
//Purpose:
//References: https://www.youtube.com/watch?v=waEsGu--9P8, https://youtu.be/8jrAWtI8RXg
//Special thanks to Code Monkeky for the tutorial!


using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Core {
    [System.Serializable]
    public sealed class GenericGrid<GridObject> {
        private GridObject[,] gridArray; //internal array
        private UnityEngine.UI.Text[,] gridLabels;
        private GameObject labelPrefab;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int CellSize { get; private set; }

        public GenericGrid (int width, int height, int cellSize, GameObject label) {
            labelPrefab = label;
            Width = width;
            Height = height;
            CellSize = cellSize;

            gridArray = new GridObject[width, height];

            //grid labels
            if (labelPrefab != null) {
                gridLabels = new UnityEngine.UI.Text[width, height];
                for (int y = 0 ; y < height ; y++) {
                    for (int x = 0 ; x < width ; x++) {
                        var o = Object.Instantiate (labelPrefab);
                        o.transform.position = GetWorldPosition (x, y, 1);
                        gridLabels[x, y] = o.GetComponentInChildren<UnityEngine.UI.Text> ();
                        gridLabels[x, y].text = $"{x},{y}";
                    }
                }
            }
        }

        public GenericGrid (int width, int height, int cellSize) {
            this.Width = width;
            this.Height = height;
            this.CellSize = cellSize;

            gridArray = new GridObject[width, height];
        }

        public int NumElements {
            get {
                return gridArray.Length;
            }
        }

        public Vector2Int Bounds {
            get {
                return new Vector2Int (Width, Height);
            }
        }

        public Vector3 GetWorldPosition (Vector2Int coords) {
            return new Vector3 (coords.x * CellSize, 0, coords.y * CellSize);
        }

        public Vector3 GetWorldPosition (int x, int y) {
            return new Vector3 (x * CellSize, 0, y * CellSize);
        }

        public Vector3 GetWorldPosition (int x, int y, int elevation) {
            return new Vector3 (x * CellSize, elevation * CellSize, y * CellSize);
        }

        public Vector3 GetWorldPositionV2 (Vector2Int coords) {
            return new Vector2 (coords.x * CellSize, coords.y * CellSize);
        }

        public Vector3 GetWorldPositionV2 (int x, int y) {
            return new Vector2 (x * CellSize, y * CellSize);
        }

        public Vector2Int GetXY (Vector3 worldPosition) {
            return new Vector2Int (
                Mathf.FloorToInt (worldPosition.x / CellSize),
                Mathf.FloorToInt (worldPosition.z / CellSize)
                );
        }

        public Vector3 SnapToGrid (Vector3 position) {
            return new Vector3 (
                      Mathf.FloorToInt (position.x / CellSize),
                      0,
                      Mathf.FloorToInt (position.z / CellSize)
                      );
        }

        public void SetValue (int x, int y, GridObject value) {
            if (x >= 0 && y >= 0 && x < Width && y < Height) {
                gridArray[x, y] = value;
            }
        }

        public void SetValue (Vector2Int coords, GridObject value) {
            if (coords.x >= 0 && coords.y >= 0 && coords.x < Width && coords.y < Height) {
                gridArray[coords.x, coords.y] = value;
            }
        }

        public void SetValue (Vector3 worldPosition, GridObject value) {
            Vector2Int coords = GetXY (worldPosition);
            SetValue (coords.x, coords.y, value);
        }

        public GridObject GetValue (int x, int y) {
            if (x >= 0 && y >= 0 && x < Width && y < Height) {
                return gridArray[x, y];
            }
            else return default;
        }

        public GridObject this[int x, int z] {
            get {
                return gridArray[x, z];
            }
            set {
                gridArray[x, z] = value;
            }
        }

        public GridObject GetValue (Vector2Int coords) {
            if (coords.x >= 0 && coords.y >= 0 && coords.x < Width && coords.y < Height) {
                return gridArray[coords.x, coords.y];
            }
            else return default;
        }

        public GridObject GetValue (Vector3 worldPosition) {
            Vector2Int coords = GetXY (worldPosition);
            return GetValue (coords.x, coords.y);
        }

        public void SetLabelColor (int x, int y, Color c) {
            if (gridLabels != null) {
                if (x >= 0 && y >= 0 && x < Width && y < Height) {
                    gridLabels[x, y].color = c;
                }
            }
        }

        public void ShowLabel (int x, int y, bool state) {
            if (gridLabels != null) {
                if (x >= 0 && y >= 0 && x < Width && y < Height) {
                    gridLabels[x, y].GetComponentInParent<Canvas> ().enabled = state;
                }
            }
        }

        public void SetLabelPosition (int x, int y, Vector3 position) {
            if (gridLabels != null) {
                if (x >= 0 && y >= 0 && x < Width && y < Height) {
                    gridLabels[x, y].transform.root.position = position;
                }
            }
        }

        public void SetLabel (int x, int y, string label) {
            if (gridLabels != null) {
                if (x >= 0 && y >= 0 && x < Width && y < Height) {
                    gridLabels[x, y].text = label;
                }
            }
        }

        public void DestroyLabels () {
            foreach (UnityEngine.UI.Text t in gridLabels) {
                Object.Destroy (t.transform.root);
            }
            gridLabels = null;
        }

        public void GenerateLebels () {
            if (gridLabels == null) {
                if (labelPrefab != null) {
                    gridLabels = new UnityEngine.UI.Text[Width, Height];
                    for (int y = 0 ; y < Height ; y++) {
                        for (int x = 0 ; x < Width ; x++) {
                            var o = Object.Instantiate (labelPrefab);
                            o.transform.position = GetWorldPosition (x, y, 1);
                            gridLabels[x, y] = o.GetComponentInChildren<UnityEngine.UI.Text> ();
                            gridLabels[x, y].text = $"{x},{y}";
                        }
                    }
                }
            }
        }

        public List<GridObject> GetNeighbourList (Vector3 worldPosition) {
            return GetNeighbourList (GetXY (worldPosition));
        }

        public List<GridObject> GetNeighbourList (int x, int y) {
            return GetNeighbourList (new Vector2Int (x, y));
        }

        public List<GridObject> GetNeighbourList (Vector2Int coords) {
            List<GridObject> neighbourList = new List<GridObject> (4);
            //left
            if (coords.x - 1 >= 0) {
                neighbourList.Add (GetValue (coords.x - 1, coords.y));
            }
            //right
            if (coords.x + 1 < Width) {
                neighbourList.Add (GetValue (coords.x + 1, coords.y));
            }
            //up
            if (coords.y - 1 >= 0) neighbourList.Add (GetValue (coords.x, coords.y - 1));
            //down
            if (coords.y + 1 < Height) neighbourList.Add (GetValue (coords.x, coords.y + 1));

            return neighbourList;
        }
        public GridObject GetRandom () {
            return gridArray[Random.Range (0, Width), Random.Range (0, Height)];
        }
    }
}
