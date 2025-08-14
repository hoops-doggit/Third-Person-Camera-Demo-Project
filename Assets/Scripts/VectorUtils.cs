using UnityEngine;

namespace DefaultNamespace {
    public static class VectorUtils {
        public static Vector3 XcY(this Vector2 v2, float c = 0f) {
            return new Vector3(v2.x, 0, v2.y);
        }
        
        public static Vector2 XZ(this Vector3 v3) {
            return new Vector2(v3.x, v3.z);
        }
        
        public static Vector2 XY(this Vector3 v3) {
            return new Vector2(v3.x, v3.y);
        }
    }
}