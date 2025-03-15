
using System.Collections.Generic;

namespace Citybuilder.Testing {
    public class CommandStack {

        private Stack<System.Action> stack;

        public CommandStack () {
            stack = new Stack<System.Action> ();
        }

        public int Count {
            get {
                return Count;
            }
        }

        public void Push (System.Action command) {
            stack.Push (command);
        }

        public void Pop () {
            if (stack.Count > 0) {
                var _ = stack.Pop ();
                _?.Invoke ();
            }
        }

        public void Clear () {
            stack.Clear ();
        }
    }
}
