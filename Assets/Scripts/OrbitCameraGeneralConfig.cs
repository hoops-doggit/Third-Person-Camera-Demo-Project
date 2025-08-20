using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCameraGeneralSettings", menuName = "Scriptable Objects/PlayerCameraGeneralSettings")]
public class OrbitCameraGeneralConfig : ScriptableObject {
    [Range(0,30)]    
    [SerializeField] private float distanceDecayRate;
    [SerializeField] private float maxPitch;
    [SerializeField] private float minPitch;
    [Range(0f, 1f)]
    [SerializeField] private float cameraSpeed;
    
    public float MaxPitch => maxPitch;
    public float MinPitch => minPitch;
    public float CameraSpeed => cameraSpeed;
    public float DistanceDecay => distanceDecayRate;
}
