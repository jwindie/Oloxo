using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Roads {
    public class RoadNode {
        public Vector3 position;
        public List<RoadNode> connections = new List<RoadNode> ();
    }
}
