using UnityEngine;

namespace DefaultNamespace {
    public class YawHint : MonoBehaviour {
        [SerializeField] private float desiredYaw;
        [SerializeField] private float decayRate = 2;

        public void OverrideYaw(ref CameraParams @params, ref float inOutYaw) {
            float delta = Mathf.DeltaAngle(inOutYaw, desiredYaw);
            
            float result = inOutYaw.ExponentialDecay(inOutYaw + delta, decayRate, Time.deltaTime);
            
            inOutYaw = Mathf.Repeat(result, 360f);
        }

        private void OnDrawGizmosSelected() {
            Vector3 origin = transform.position;
            Matrix4x4 matrix = Matrix4x4.TRS(origin, Quaternion.Euler(0,desiredYaw,0), Vector3.one);
            
            Vector3 arrowPoint = Vector3.forward;
            arrowPoint = matrix.MultiplyPoint3x4(arrowPoint);
            
            Vector3 leftArrowHead = Vector3.forward * 0.75f + Vector3.left * 0.25f;
            leftArrowHead = matrix.MultiplyPoint3x4(leftArrowHead);
            
            Vector3 rightArrowHead = Vector3.forward * 0.75f + Vector3.right * 0.25f;
            rightArrowHead = matrix.MultiplyPoint3x4(rightArrowHead);
            
            Gizmos.DrawLine(origin,  arrowPoint);
            Gizmos.DrawLine(arrowPoint, rightArrowHead);
            Gizmos.DrawLine(arrowPoint, leftArrowHead);
        }
    }
}