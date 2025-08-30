using UnityEngine;

namespace ThisNamespace {
    public interface IVirtualCamera {
        string Name { get; }
        int Priority { get; set; }
        Vector3 Position();
        Quaternion Rotation();
        float FieldOfView();
        void UpdateCamera();
        void Activate(PreviousCameraInfo info);
        void Deactivate();
        bool Active { get; }
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

        public PreviousCameraInfo() {
            Name = string.Empty;
        }
    }
}
