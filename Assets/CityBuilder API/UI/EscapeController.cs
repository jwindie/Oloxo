using Citybuilder.Testing;

namespace Citybuilder.UI {
    public class EscapeController : Singleton<EscapeController> {

        private CommandStack commandStack = new CommandStack ();

        public int Count {
            get {
                return commandStack.Count;
            }
        }

        public void AddEscapeAction (System.Action command) {
            commandStack.Push (command);
        }

        public void HandleEscape () {
            commandStack.Pop ();
        }

        public void Clear () {
            commandStack.Clear ();
        }

    }
}