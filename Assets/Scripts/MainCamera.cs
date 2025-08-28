using UnityEngine;

namespace ThisNamespace {
    public class MainCamera : Singleton<MainCamera> {
        [SerializeField] private new Transform transform;

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
            transform.SetPositionAndRotation(position, rotation);
        }
    }
}