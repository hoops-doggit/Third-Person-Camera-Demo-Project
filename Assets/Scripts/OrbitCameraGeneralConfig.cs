using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCameraGeneralSettings", menuName = "Scriptable Objects/PlayerCameraGeneralSettings")]
public class OrbitCameraGeneralConfig : ScriptableObject {
    [Range(0,30)]    
    [SerializeField] private float distanceDecayRate;
    [SerializeField] private float maxPitch;
    [SerializeField] private float minPitch;
    [Range(0f, 1f)]
    [SerializeField] private float cameraSpeed;

    [Header("Yaw Gravity")]
    [SerializeField] private float yawGravityVelocityThreshold;
    [Tooltip("Delay (in seconds) after no input received before we allow yaw gravity to take effect")]
    [SerializeField] private float yawGravityDelay;
    [Range(0.01f, 1f)] 
    [SerializeField] private float yawGravityAcceleration;
    [Range(0.01f,40f)]
    [SerializeField] private float yawGravityDeceleration;
    [SerializeField] private float yawGravityStrength;
    [SerializeField] private float yawGravityDeadZone;
    
    public float MaxPitch => maxPitch;
    public float MinPitch => minPitch;
    public float CameraSpeed => cameraSpeed;
    public float DistanceDecay => distanceDecayRate;
    
    public float YawGravityVelocityThreshold => yawGravityVelocityThreshold;
    public float YawGravityDelay => yawGravityDelay;
    public float YawGravityAcceleration => yawGravityAcceleration;
    public float YawGravityDeceleration => yawGravityDeceleration;
    public float YawGravityStrength => yawGravityStrength;
    public float YawGravityDeadZone => yawGravityDeadZone;
}
