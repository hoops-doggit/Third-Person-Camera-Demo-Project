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
    }
}