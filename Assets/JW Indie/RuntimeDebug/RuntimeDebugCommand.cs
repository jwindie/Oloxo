//Author: Jordan Williams
//Date: Sep 16 2020

using System;

namespace JWIndie.RuntimeDebugger {
    public class RuntimeDebugCommand : RuntimeDebugCommandBase {
        private Action command;

        public RuntimeDebugCommand (string id, string description, string format, Action command) : base (id, description, format) {
            this.command = command;
        }

        public void Invoke () {
            command.Invoke ();
        }
    }

    public class RuntimeDebugCommand<T1> : RuntimeDebugCommandBase {
        private Action<T1> command;

        public RuntimeDebugCommand (string id, string description, string format, Action<T1> command) : base (id, description, format) {
            this.command = command;
        }

        public void Invoke (T1 value) {
            command.Invoke (value);
        }
    }

    public class RuntimeDebugCommand<T1, T2> : RuntimeDebugCommandBase {
        private Action<T1, T2> command;

        public RuntimeDebugCommand (string id, string description, string format, Action<T1, T2> command) : base (id, description, format) {
            this.command = command;
        }

        public void Invoke (T1 v1, T2 v2) {
            command.Invoke (v1, v2);
        }
    }

    public class RuntimeDebugCommand<T1, T2, T3> : RuntimeDebugCommandBase {
        private Action<T1, T2, T3> command;

        public RuntimeDebugCommand (string id, string description, string format, Action<T1, T2, T3> command) : base (id, description, format) {
            this.command = command;
        }

        public void Invoke (T1 v1, T2 v2, T3 v3) {
            command.Invoke (v1, v2, v3);
        }
    }

    public class RuntimeDebugCommand<T1, T2, T3, T4> : RuntimeDebugCommandBase {
        private Action<T1, T2, T3, T4> command;

        public RuntimeDebugCommand (string id, string description, string format, Action<T1, T2, T3, T4> command) : base (id, description, format) {
            this.command = command;
        }

        public void Invoke (T1 v1, T2 v2, T3 v3, T4 v4) {
            command.Invoke (v1, v2, v3, v4);
        }
    }

    public class RuntimeDebugCommand<T1, T2, T3, T4, T5> : RuntimeDebugCommandBase {
        private Action<T1, T2, T3, T4, T5> command;

        public RuntimeDebugCommand (string id, string description, string format, Action<T1, T2, T3, T4, T5> command) : base (id, description, format) {
            this.command = command;
        }

        public void Invoke (T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            command.Invoke (v1, v2, v3, v4, v5);
        }
    }

    public class RuntimeDebugCommand<T1, T2, T3, T4, T5, T6> : RuntimeDebugCommandBase {
        private Action<T1, T2, T3, T4, T5, T6> command;

        public RuntimeDebugCommand (string id, string description, string format, Action<T1, T2, T3, T4, T5, T6> command) : base (id, description, format) {
            this.command = command;
        }

        public void Invoke (T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6) {
            command.Invoke (v1, v2, v3, v4, v5, v6);
        }
    }


    public class RuntimeDebugCommand<T1, T2, T3, T4, T5, T6, T7> : RuntimeDebugCommandBase {
        private Action<T1, T2, T3, T4, T5, T6, T7> command;

        public RuntimeDebugCommand (string id, string description, string format, Action<T1, T2, T3, T4, T5, T6, T7> command) : base (id, description, format) {
            this.command = command;
        }

        public void Invoke (T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7) {
            command.Invoke (v1, v2, v3, v4, v5, v6, v7);
        }
    }
}
