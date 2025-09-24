using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCameraGeneralSettings", menuName = "Scriptable Objects/PlayerCameraGeneralSettings")]
public class OrbitCameraGeneralConfig : ScriptableObject {
    [Range(0,30)]    
    [SerializeField] private float distanceDecayRate;
    [SerializeField] private float maxPitch;
    [SerializeField] private float minPitch;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float yawDecay = 16;
    [SerializeField] private AnimationCurve yFramingXPitch;
    [SerializeField] private AnimationCurve xFramingXPitch;
    
    
    [Header("Pitch & Yaw Gravity")]
    [Tooltip("Delay (in seconds) after no input received before we allow yaw gravity to take effect")]
    [SerializeField] private float gravityDelay;
    
    [Header("Yaw Gravity"), Tooltip("The velocity the player must be moving faster than before Yaw Gravity will kick in.")]
    [SerializeField] private float yawGravityVelocityThreshold;

    [SerializeField] private float yawGravityFrequency;
    [SerializeField] private float yawGravityDamping;
    [SerializeField] private float yawGravityResponse;
    [SerializeField] private float yawGravityStrength;
    [SerializeField] private Vector2 yawGravityMinMaxStrength;
    [SerializeField] private float yawGravityFrontDeadZone;
    [SerializeField] private float yawGravityBackDeadZone;
    [SerializeField] private AnimationCurve yawGravityCrossSampler;


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
    public Vector2 YawGravityMinMaxStrength => yawGravityMinMaxStrength;
    public float YawGravityFrontDeadZone => yawGravityFrontDeadZone;
    public float YawGravityBackDeadZone => yawGravityBackDeadZone;
    public AnimationCurve YawGravityCrossSampler => yawGravityCrossSampler;
    
    public float PitchGravityFrequency =>pitchGravityFrequency;
    public float PitchGravityDamping =>pitchGravityDamping;
    public float PitchGravityResponse =>pitchGravityResponse;
    public float PitchGravityStrength =>pitchGravityStrength;
    public float YawDecay => yawDecay;
    public AnimationCurve XFramingXPitch => xFramingXPitch;
    public AnimationCurve YFramingXPitch => yFramingXPitch;
}
