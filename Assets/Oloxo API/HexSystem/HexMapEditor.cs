using UnityEngine;
using UnityEngine.EventSystems;

namespace Oloxo.HexSystem {

    public class HexMapEditor : MonoBehaviour {

        [System.Serializable]
        public enum EditMode {
            Ignore, Add, Remove
        }

        [SerializeField] private Camera m_Camera;
        public HexGrid hexGrid;

        [Header ("Settings")]
        [Range (0, 4)] public int brushSize;
        [Space (20)]
        EditMode terrainEditMode;
        public Terrain terrain;
        [Space (20)]
        EditMode harvestedEditMode;
        public bool harvested;


        void Update () {
            if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject ()) {
                HandleInput ();
            }
        }

        void HandleInput () {
            Ray inputRay = m_Camera.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (inputRay, out hit)) {
                EditCells (hexGrid.GetCell (hit.point));
            }
        }

        void EditCells (HexCell center) {
            //get the coords of the center cell
            int centerX = center.coordinates.X;
            int centerZ = center.coordinates.Z;

            //loop through the bottom of the selection
            for (int r = 0, z = centerZ - brushSize ; z <= centerZ ; z++, r++) {
                for (int x = centerX - r ; x <= centerX + brushSize ; x++) {
                    EditCell (hexGrid.GetCell (new HexCoordinates (x, z)));
                }
            }

            //repeat for the top half
            for (int r = 0, z = centerZ + brushSize ; z > centerZ ; z--, r++) {
                for (int x = centerX - brushSize ; x <= centerX + r ; x++) {
                    EditCell (hexGrid.GetCell (new HexCoordinates (x, z)));
                }
            }

        }

        void EditCell (HexCell cell) {
            if (cell) {
                //make the changes to the cell here
                if (harvestedEditMode > EditMode.Ignore) cell.Harvested = harvestedEditMode == EditMode.Add;
                if (terrainEditMode > EditMode.Ignore)  cell.Terrain = terrain; 
            }
        }

        private void OnValidate () {
            //do not allow the editor to make some decisions.
        }
    }
}