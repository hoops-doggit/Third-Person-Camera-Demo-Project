using UnityEngine;

namespace ThisNamespace {
    public class DistanceHint : MonoBehaviour{
        [SerializeField] private float desiredDistance;
        [SerializeField] private float decayRate = 2;
        
        public void OverrideDistance(ref CameraParams @params, ref float inOutDistance) {
            inOutDistance = inOutDistance.ExponentialDecay( desiredDistance, decayRate, Time.deltaTime);
        }
    }
}