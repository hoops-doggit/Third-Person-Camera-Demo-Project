using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCameraSettings", menuName = "Scriptable Objects/PlayerCameraSettings")]
public class PlayerCameraSettings : ScriptableObject {
    [SerializeField] private float trackingHeight;
    [SerializeField] private float minDistance = 1;
    [Range(0,1)]
    [SerializeField] private float midDistance = .5f;
    [SerializeField] private float maxDistance = 7;
    [SerializeField] private float fieldOfView;
    [SerializeField] private float sphereCastRadius = .2f;
    [SerializeField] private AnimationCurve distanceXpitchCurve;
    [Range(-1,1)]
    [SerializeField] private float xFraming, yFraming;
    
    public float TrackingHeight => trackingHeight;
    public float MinDistance => minDistance;
    public float MidDistance => Mathf.Lerp(minDistance, maxDistance, midDistance);
    public float MaxDistance => maxDistance;
    public float FieldOfView => fieldOfView;
    public float SphereCastRadius => sphereCastRadius;
    public AnimationCurve DistanceXPitchCurve => distanceXpitchCurve;
    public float XFraming => xFraming;
    public float YFraming => yFraming;
}
