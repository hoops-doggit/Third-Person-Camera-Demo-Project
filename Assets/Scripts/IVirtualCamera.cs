using UnityEngine;

namespace ThisNamespace {
    public interface IVirtualCamera {
        public string Name { get; }
        public Vector3 Position();
        public Quaternion Rotation();
        public float FieldOfView();
        public void UpdateCamera();
        int Priority { get; set; }
        void Activate(PreviousCameraInfo info);
        void Deactivate();
    }

    public class PreviousCameraInfo {
        public string Name;
        public Vector3 Position;
        public Quaternion Rotation;
        public float FieldOfView;
        public bool PlayerCameraShouldMatch;

        public PreviousCameraInfo(IVirtualCamera camera) {
            Name = camera.Name;
            Position = camera.Position();
            Rotation = camera.Rotation();
            FieldOfView = camera.FieldOfView();
        }
    }
}
