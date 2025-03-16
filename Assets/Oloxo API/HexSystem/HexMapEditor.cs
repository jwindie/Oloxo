using UnityEngine;
using UnityEngine.EventSystems;

namespace Oloxo.HexSystem {

    public class HexMapEditor : MonoBehaviour {

        [SerializeField] private Camera m_Camera;
        public bool harvested;
        public HexGrid hexGrid;

        void Update () {
            if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject ()) {
                HandleInput ();
            }
        }

        void HandleInput () {
            Ray inputRay = m_Camera.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (inputRay, out hit)) {
                hexGrid.HarvestCell (hit.point, harvested);
            }
        }
    }
}