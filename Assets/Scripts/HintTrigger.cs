using DefaultNamespace;
using UnityEngine;

namespace ThisNamespace {
    public class HintTrigger : MonoBehaviour {
        [SerializeField] private PitchHint pitchHint = null;
        [SerializeField] private YawHint[] yawHints = null;
        [SerializeField] private DistanceHint distHint = null;
        
        private OrbitCamera _orbitCamera = null;
        private bool _beenActivated = false;
        
        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player")) {
                return;
            }
            
            if (!other.TryGetComponent(out PlayerHub hub)) {
               return;
            }

            if (hub.OrbitCamera == null) {
                return;
            }
            
            _beenActivated = false;
            _orbitCamera = hub.OrbitCamera;
            if (PlayerInput.Instance.Look.sqrMagnitude > 0.25f * 0.25f) {
                return;
            }
            ActivateHint();
            _beenActivated = true;
        }

        private void ActivateHint() {
            if (pitchHint != null) {
                _orbitCamera.OverridePitch += pitchHint.OverridePitch;
            }
                
            Vector3 playerPos = _orbitCamera.transform.position;
            if (yawHints != null && yawHints.Length > 0) {
                YawHint closest = GetClosestYawHint(playerPos);
                _orbitCamera.OverrideYaw += closest.OverrideYaw;
            }

            if (distHint != null) {
                _orbitCamera.OverrideDist += distHint.OverrideDistance;
            }
        }

        private void OnTriggerStay(Collider other) {
            if (!_beenActivated && _orbitCamera != null) {
                _beenActivated = true;
                ActivateHint();
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.TryGetComponent(out ComponentHub hub)) {
                if (hub.OrbitCamera != null && hub.OrbitCamera == _orbitCamera) {
                    Deactivate(_orbitCamera);
                }
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

            if (distHint != null) {
                cam.OverrideDist -= distHint.OverrideDistance;
            }
        }


        private YawHint GetClosestYawHint(Vector3 playerPos) {
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