using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCameraGeneralSettings", menuName = "Scriptable Objects/PlayerCameraGeneralSettings")]
public class OrbitCameraGeneralConfig : ScriptableObject {
    [Range(0,30)]    
    [SerializeField] private float distanceDecayRate;
    [SerializeField] private float maxPitch;
    [SerializeField] private float minPitch;
    [Range(0f, 1f)]
    [SerializeField] private float cameraSpeed;

    [Header("Pitch & Yaw Gravity")]
    [Tooltip("Delay (in seconds) after no input received before we allow yaw gravity to take effect")]
    [SerializeField] private float gravityDelay;
    
    [Header("Yaw Gravity"), Tooltip("The velocity the player must be moving faster than before Yaw Gravity will kick in.")]
    [SerializeField] private float yawGravityVelocityThreshold;

    [SerializeField] private float yawGravityFrequency;
    [SerializeField] private float yawGravityDamping;
    [SerializeField] private float yawGravityResponse;
    [SerializeField] private float yawGravityStrength;
    [SerializeField] private float yawGravityDeadZone;

    [Header("Pitch Gravity")] 
    [SerializeField] private float pitchGravityDelay;
    [SerializeField] private float pitchGravityFrequency;
    [SerializeField] private float pitchGravityDamping;
    [SerializeField] private float pitchGravityResponse;
    [SerializeField] private float pitchGravityStrength;
    
    public float MaxPitch => maxPitch;
    public float MinPitch => minPitch;
    public float CameraSpeed => cameraSpeed;
    public float DistanceDecay => distanceDecayRate;
    
    public float YawGravityVelocityThreshold => yawGravityVelocityThreshold;
    public float GravityDelay => gravityDelay;
    public float YawGravityFrequency => yawGravityFrequency;
    public float YawGravityDamping => yawGravityDamping;
    public float YawGravityResponse => yawGravityResponse;
    public float YawGravityStrength => yawGravityStrength;
    public float YawGravityDeadZone => yawGravityDeadZone;
    
    public float PitchGravityFrequency =>pitchGravityFrequency;
    public float PitchGravityDamping =>pitchGravityDamping;
    public float PitchGravityResponse =>pitchGravityResponse;
    public float PitchGravityStrength =>pitchGravityStrength;
}
