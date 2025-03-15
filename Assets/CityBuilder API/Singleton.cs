using UnityEngine;

namespace Citybuilder {
    public class Singleton<T> : MonoBehaviour where T : Component {
        private static T instance;

        public static T Current {
            get {
                if (instance == null) {
                    var instances = FindObjectsOfType<T> ();

                    if (instances.Length == 1) {
                        instance = instances[0];
                    }
                    else if (instances.Length > 1) {
                        UnityEngine.Debug.LogError (typeof (T) + ": There is more than 1 instance in the scene.");
                    }
                    else {
                        UnityEngine.Debug.LogError (typeof (T) + ": Instance doesn't exist in the scene.");
                    }
                }
                return instance;
            }
        }
    }
}
