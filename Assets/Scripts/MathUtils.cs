using UnityEngine;

namespace ThisNamespace {
    public static class MathUtils {
        public static int Wrap(int x, int min, int max) {
            return (x < min) ? min : (x > max) ? max : x;
        }
        
        public static float Wrap(float x, float min, float max) {
            if (x > max) {
                x -= max;
            }

            else if (x < min) {
                x += max;
            }
            return x;
        }
        
        // Check out Freya Holmer's video on interpolation over time
        public static float ExponentialDecay(this float current, float target, float decay, float deltaTime) {
            return target + (current - target) * Mathf.Exp(-decay * deltaTime);
        }

        public static float Closest(float value, float min, float max) {
            float midPoint = (min + max) * 0.5f;
            return value < midPoint ? min : max;
        }
        
        public static float SafeDivide(float numerator, float denominator) {
            if (denominator == 0) {
                return 0;
            }
            return numerator/denominator;
        }
        

        /// This assumes the magnitude of the two vectors that you want to calculate the cross product from are both unit vectors
        public static float CrossProductMagnitudeFromAngle(float angleInDegrees)
        {
            // Convert angle from degrees to radians
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        
            // Calculate the sine of the angle
            return Mathf.Sin(angleInRadians);
        }
        
        public static float DotProductMagnitudeFromAngle(float angleInDegrees)
        {
            // Convert angle from degrees to radians
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        
            // Calculate the cosine of the angle
            return Mathf.Cos(angleInRadians);
        }
    }
}