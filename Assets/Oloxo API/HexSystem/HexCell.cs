using UnityEngine;

namespace Oloxo.HexSystem {

    public class HexCell : MonoBehaviour {
        //Cells do not need to be anything but data and do not need to inherit from
        //MonoBehaviour. 

        public HexCoordinates coordinates;
        public bool harvested;
    }
}