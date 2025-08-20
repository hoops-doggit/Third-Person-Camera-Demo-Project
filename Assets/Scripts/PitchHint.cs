using UnityEngine;

namespace DefaultNamespace {
    public class PitchHint : MonoBehaviour {
        [SerializeField] private Vector2 desiredPitchRange;
        [SerializeField] private float decayRate;

        public void OverridePitch(ref CameraParams @params, ref float inOutPitch) {
            float closest;
            
            // This is unnecessary if you have editor code validating your range;
            if (desiredPitchRange.x < desiredPitchRange.y) {
                closest = MathUtils.Closest(inOutPitch, desiredPitchRange.x, desiredPitchRange.y);
            } else {
                closest = MathUtils.Closest(inOutPitch, desiredPitchRange.y, desiredPitchRange.x);
            }
            
            inOutPitch = inOutPitch.ExponentialDecay(closest, decayRate, Time.deltaTime);
        }
    }
}