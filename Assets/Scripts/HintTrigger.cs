using UnityEngine;

namespace DefaultNamespace {
    public class HintTrigger : MonoBehaviour {
        [SerializeField] private PitchHint pitchHint = null;
        [SerializeField] private YawHint[] yawHints = null;
        
        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out OrbitCamera cam)) {
                Vector3 playerPos = cam.transform.position;
                
                if (pitchHint != null) {
                    cam.OverridePitch += pitchHint.OverridePitch;
                }
                
                if (yawHints != null) {
                    YawHint closest = GetClosestYawhint(playerPos);
                    cam.OverrideYaw += closest.OverrideYaw;
                }
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent(out OrbitCamera cam)) {
                if (pitchHint != null) {
                    cam.OverridePitch -= pitchHint.OverridePitch;
                }

                if (yawHints != null) {
                    foreach (var yawHint in yawHints) {
                        cam.OverrideYaw -= yawHint.OverrideYaw;
                    }
                }
            }
        }

        private YawHint GetClosestYawhint(Vector3 playerPos) {
            float closestDistance = float.MaxValue;
            YawHint closest = null;
            foreach (var hint in yawHints) {
                float distance = Vector3.Distance(playerPos, hint.transform.position);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closest = hint;
                }
            }

            return closest;
        }
    }
}