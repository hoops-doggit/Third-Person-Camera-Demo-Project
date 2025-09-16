using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCameraSettings", menuName = "Scriptable Objects/PlayerCameraSettings")]
public class OrbitCameraSettingsConfig : ScriptableObject {
    [SerializeField] private string displayName;
    [SerializeField] private float trackingHeight;
    [SerializeField] private float minDistance = 1;
    [Range(0,1)]
    [SerializeField] private float midDistance = .5f;
    [SerializeField] private float maxDistance = 7;
    [SerializeField] private float fieldOfView;
    [SerializeField] private float sphereCastRadius = .2f;
    [SerializeField] private AnimationCurve distanceXpitchCurve;
    [SerializeField] private AnimationCurve fieldOfViewXpitchCurve;
    [Range(-1,1)]
    [SerializeField] private float xFraming, yFraming;

    [Header("Preferred Ranges")] 
    [SerializeField] private Vector2 minMaxPreferredPitch;
    
    public float TrackingHeight => trackingHeight;
    public float MinDistance => minDistance;
    public float MidDistance => Mathf.Lerp(minDistance, maxDistance, midDistance);
    public float MaxDistance => maxDistance;
    public float FieldOfView => fieldOfView;
    public float SphereCastRadius => sphereCastRadius;
    public AnimationCurve DistanceXPitchCurve => distanceXpitchCurve;
    public AnimationCurve FieldOfViewXPitchCurve => fieldOfViewXpitchCurve;
    public float XFraming => xFraming;
    public float YFraming => yFraming;
    public Vector2 MinMaxPreferredPitch => minMaxPreferredPitch;
}
