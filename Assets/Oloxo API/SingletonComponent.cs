using UnityEngine;

namespace Oloxo {

    public abstract class SingletonComponent<T> : MonoBehaviour where T : Component {
        protected static T instance;

        public static T Current {
            get {
                return instance;
            }
        }

        public static void SetSingletonInstance (T item) {
            instance = item;
        }
    }
}