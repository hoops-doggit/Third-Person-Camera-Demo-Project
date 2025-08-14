using UnityEngine;

namespace DefaultNamespace {
    public interface IVirtualCamera {
        public Vector3 Position();
        public Quaternion Rotation();
        public float FieldOfView();
        public void UpdateCamera();
    }
}