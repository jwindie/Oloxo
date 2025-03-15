using Citybuilder.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Citybuilder.Roads {
    public class RoadNetwork : Singleton<RoadNetwork> {

        private HashSet<RoadNode> nodes;

        public RoadNetwork () {
            nodes = new HashSet<RoadNode> ();
        }

        public void OnDrawGizmos () {
            Gizmos.color = Color.blue;
            foreach (RoadNode node in nodes) {
                Gizmos.DrawCube (node.position, Vector3.one * .2f);

                foreach (RoadNode connection in node.connections) {
                    Gizmos.DrawLine (node.position, connection.position);
                }
            }
        }
    }

}
