using UnityEngine;

namespace DefaultNamespace {
    public class Singleton<T> : MonoBehaviour where T : class {
        public static T Instance {get; private set;}
        public static bool Exists => Instance != null;

        protected virtual void Awake() {
            if (Instance != null && Instance as UnityEngine.Object != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this as T;
        }
    }
}