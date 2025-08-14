using UnityEngine;

namespace DefaultNamespace {
    
    public struct CameraParams {
        public Vector3 trackingPoint;    // 3D world position of the point camera orbits around
        public Vector2 framing;             // 2D screen position of the trackingPosition
        public float distance;              // offset between the camera and the player
        public float pitch;                 // vertical camera tilt
        public float yaw;                   // horizontal camera pan
    }
    public static class CameraUtils {
        
        // Apply CameraParams to Camera
        public static void SetParams(this Camera camera, in CameraParams camParams) {
            // compute "local" offset relative to our view rotation
            float tanFOVY = Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
            float tanFOVX = tanFOVY * camera.aspect;
            Vector3 localOffset = new Vector3(
                camParams.distance * tanFOVX * camParams.framing.x,
                camParams.distance * tanFOVY * camParams.framing.y,
                camParams.distance
            );

            // compute position and rotation
            Quaternion rotation = Quaternion.Euler(camParams.pitch, camParams.yaw, 0);
            Vector3 position = camParams.trackingPoint - rotation * localOffset;
            camera.transform.SetPositionAndRotation(position, rotation);
        }
        
        public static Location SetParams(float fov, in CameraParams camParams, float aspectRatio) {
            // compute "local" offset relative to our view rotation
            float tanFOVY = Mathf.Tan(0.5f * Mathf.Deg2Rad * fov);
            float tanFOVX = tanFOVY * aspectRatio;
            Vector3 localOffset = new Vector3(
                camParams.distance * tanFOVX * camParams.framing.x,
                camParams.distance * tanFOVY * camParams.framing.y,
                camParams.distance
            );

            // compute position and rotation
            Quaternion rotation = Quaternion.Euler(camParams.pitch, camParams.yaw, 0);
            Vector3 position = camParams.trackingPoint - rotation * localOffset;
            return new Location(position, rotation);
        }
        
        // Constructs Params From A Camera
        public static void GetParams(this Camera camera, out CameraParams result, in Vector3 trackingPosition) {
            Vector3 position = camera.transform.position;
            Quaternion rotation = camera.transform.rotation;

            // tracking position
            result.trackingPoint = trackingPosition;

            // pitch/yaw
            Vector3 eulerAngles = rotation.eulerAngles;
            result.pitch = eulerAngles.x;
            result.yaw = eulerAngles.y;
    
            // distance
            Vector3 toTrackingOff = trackingPosition - position;
            Vector3 fwd = rotation * Vector3.forward;
            result.distance = Vector3.Dot(toTrackingOff, fwd);
    
            // framing
            Vector3 toCameraOff = position - trackingPosition;
            Vector3 parallax = Quaternion.Inverse(rotation) * toCameraOff;
            float tanFOVY = Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
            float tanFOVX = tanFOVY * camera.aspect;
            Vector2 screenToWorld = result.distance * new Vector2(tanFOVX, tanFOVY);
            result.framing.x = -parallax.x / screenToWorld.x;
            result.framing.y = -parallax.y / screenToWorld.y;
        }
        
        public struct CameraBlend {
            public Vector2 framing;
            public float pitch;
            public float yaw;
            public float distance;
        }

        // Blends a Camera's current position with a new one 
        public static void ComputeBlend(this Camera camera, out CameraBlend result, in CameraParams newParams) {
            camera.GetParams(out CameraParams oldParams, in newParams.trackingPoint);

            // offset from the new target to the current view
            result.framing = oldParams.framing - newParams.framing;
            result.pitch = oldParams.pitch - newParams.pitch;
            result.yaw = oldParams.yaw - newParams.yaw;
            result.distance = oldParams.distance - newParams.distance;

            // yaw can wrap-around
            if (result.yaw > 180f) { result.yaw -= 360f; }
            else if (result.yaw < -180f) { result.yaw += 360f; }
        }

        public static void SetParamsBlended(this Camera camera, in CameraParams camParams, in CameraBlend blend, float progress) {
            float multi = 1f - Mathf.Clamp01(progress);

            CameraParams result = camParams;
            result.framing += multi * blend.framing;
            result.pitch += multi * blend.pitch;
            result.yaw += multi * blend.yaw;
            result.distance += multi * blend.distance;

            camera.SetParams(in result);
        }
        

    }
    
    
}