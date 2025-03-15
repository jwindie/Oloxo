//Author: Jordan Williams
//Date: Sep 16 2020

namespace JWIndie.RuntimeDebugger {
    public class RuntimeDebugCommandBase {
        private string _commandId;
        private string _commandDescription;
        private string _commandFormat;

        public string CommandId {
            get {
                return _commandId;
            }
        }
        public string CommandDescription {
            get {
                return _commandDescription;
            }
        }

        public string CommandFormat {
            get {
                return _commandFormat;
            }
        }

        public RuntimeDebugCommandBase (string id, string description, string format) {
            _commandId = id;
            _commandDescription = description;
            _commandFormat = format;
        }
    }
}
