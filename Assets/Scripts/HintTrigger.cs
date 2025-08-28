using UnityEngine;

namespace ThisNamespace {
    public class HintTrigger : MonoBehaviour {
        [SerializeField] private PitchHint pitchHint = null;
        [SerializeField] private YawHint[] yawHints = null;
        // Serialized for debug visibility
        [SerializeField] private bool begunHint = false;
        
        private OrbitCamera _orbitCamera = null;
        private bool _beenActivated = false;
        
        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent(out OrbitCamera cam)) {
                _beenActivated = false;
                _orbitCamera = cam;
                if (PlayerInput.Instance.Look.magnitude != 0) {
                    return;
                }
                ActivateHint();
                _beenActivated = true;
            }
        }

        private void ActivateHint() {
            if (pitchHint != null) {
                _orbitCamera.OverridePitch += pitchHint.OverridePitch;
            }
                
            Vector3 playerPos = _orbitCamera.transform.position;
            if (yawHints != null) {
                YawHint closest = GetClosestYawhint(playerPos);
                _orbitCamera.OverrideYaw += closest.OverrideYaw;
            }
        }

        private void OnTriggerStay(Collider other) {
            if (!_beenActivated && _orbitCamera != null) {
                _beenActivated = true;
                ActivateHint();
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent(out OrbitCamera cam)) {
                Deactivate(cam);
            }
        }

        private void Deactivate(OrbitCamera cam) {
            if (pitchHint != null) {
                cam.OverridePitch -= pitchHint.OverridePitch;
            }

            if (yawHints != null) {
                foreach (var yawHint in yawHints) {
                    cam.OverrideYaw -= yawHint.OverrideYaw;
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