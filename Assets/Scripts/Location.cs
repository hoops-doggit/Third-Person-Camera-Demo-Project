using UnityEngine;

namespace DefaultNamespace {
    public struct Location {
        public Vector3 position;
        public Quaternion rotation;

        public Location(Vector3 position, Quaternion rotation) {
            this.position = position;
            this.rotation = rotation;
        }
    }
}