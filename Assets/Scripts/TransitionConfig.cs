using UnityEngine;

namespace ThisNamespace {
    [CreateAssetMenu(fileName = "TransitionConfig", menuName = "Scriptable Objects/Transition Config")]
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