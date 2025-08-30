using UnityEngine;

namespace ThisNamespace {
    public static class VectorUtils {
        public static Vector3 XcY(this Vector2 v2, float c = 0f) {
            return new Vector3(v2.x, c, v2.y);
        }
        
        public static Vector2 XZ(this Vector3 v3) {
            return new Vector2(v3.x, v3.z);
        }

        public static Vector3 XcZ(this Vector3 v3, float c = 0f) {
            return new Vector3(v3.x, c, v3.y);
        }
        
        public static Vector2 XY(this Vector3 v3) {
            return new Vector2(v3.x, v3.y);
        }

        // Returns angle between -180 and 180 degrees, where Z+ is 0 degrees, X+ is 90
        public static float AngleXZ(this Vector3 dir) {
            return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        }
        
        public static float YawDeg360(this Vector3 dir) {
            float a = AngleXZ(dir);
            return a < 0f ? a + 360f : a;
        }
    }
}