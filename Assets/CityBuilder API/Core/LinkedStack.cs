using System.Collections.Generic;

namespace Citybuilder.Core {
    public class LinkedStack<Item> {


        private LinkedList<Item> internalLinkedList;

        public LinkedStack () {
            internalLinkedList = new LinkedList<Item> ();
        }

        public int Count {
            get {
                return internalLinkedList.Count;
            }
        }

        public LinkedList<Item> GetInternalLinkedList {
            get {
                return internalLinkedList;
            }
        }

        public void Clear () {
            internalLinkedList.Clear ();
        }

        public void Push (Item item) {
            internalLinkedList.AddLast (item);
        }

        public Item Pop () {
            var lastItem = internalLinkedList.Last.Value;
            internalLinkedList.RemoveLast ();
            return lastItem;
        }


        public Item Peek () {
            return internalLinkedList.Last.Value;
        }


        public void RemoveFirst () {
            internalLinkedList.RemoveFirst ();
        }
    }
}
