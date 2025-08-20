using UnityEngine;

namespace DefaultNamespace {
    public interface IOrbitCameraHint {
        void OverrideCamTracking(ref CameraParams Cam, ref Vector3 InOutTrackingPoint);
        void OverrideCamFraming(ref CameraParams Cam, ref Vector2 InOutFraming);
        void OverrideCamPitch(ref CameraParams Cam, ref float InOutPitch);
        void OverrideCamYaw(ref CameraParams Cam, ref float InOutYaw);
        void OverrideCamDist(ref CameraParams Cam, ref float InOutDist);
    }
}