using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citybuilder.AI {
    public class Node {

        public int clearKey;
        public int g;
        public int h;
        public Node parent;
        public int x { get; }
        public int z { get; }
        public int f { get { return g + h; } }
        public Node (int x, int z) {
            this.x = x;
            this.z = z;
        }
        public LinkedListNode<Node> linkedListNode;
    }
}
