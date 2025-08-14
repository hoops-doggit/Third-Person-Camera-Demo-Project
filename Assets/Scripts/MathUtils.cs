namespace DefaultNamespace {
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
    }
}