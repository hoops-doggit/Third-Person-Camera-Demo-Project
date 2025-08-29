using UnityEngine;

namespace ThisNamespace {
    public class TransitionConfig : ScriptableObject {
        public TransitionInfo[] transitions;

        public bool GetTransition(string camAName, string camBName, out TransitionInfo transition) {
            foreach (TransitionInfo info in transitions) {
                if (info.FromCameraName == camAName && info.ToCameraName == camBName) {
                    transition = info;
                    return true;
                }
            }

            transition = null;
            return false;
        }
    }
}