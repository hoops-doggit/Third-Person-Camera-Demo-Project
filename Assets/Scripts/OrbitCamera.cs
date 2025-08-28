using ThisNamespace;
using UnityEngine;

public class OrbitCamera : MonoBehaviour, IVirtualCamera
{
    [SerializeField] private OrbitCameraGeneralConfig generalConfig;
    [SerializeField] private OrbitCameraSettingsConfig initialSettings;
    [SerializeField] private LayerMask cameraObstacleMask;
    [SerializeField] private PlayerMovement movement;
    
    private OrbitCameraSettingsConfig _currentSettings;
    [SerializeField] private Transform _transform;
    private Location _location;
    
    // * Persistant state
    private int _distanceZone = 1;   
    [SerializeField] // just so we can see it in editor
    private CameraParams _cameraParams;
    //

    // PointOFView
    public CameraParams CameraParams => _cameraParams;
    public string Name => "PlayerCamera"; 
    public Vector3 Position() => _location.position;
    public Quaternion Rotation() => _location.rotation;
    public float FieldOfView() => _currentSettings.FieldOfView * _currentSettings.FieldOfViewXPitchCurve.Evaluate(_cameraParams.pitch);
    
    
    public delegate void OverrideCameraTracking(ref CameraParams Cam, ref Vector3 InOutTrackingPoint);
    public OverrideCameraTracking OverrideTracking { get; set; }
    public delegate void OverrideCameraFraming(ref CameraParams Cam, ref Vector2 InOutFraming);
    public OverrideCameraFraming OverrideFraming { get; set; }
    public delegate void OverrideCameraPitch(ref CameraParams Cam, ref float InOutPitch);
    public OverrideCameraPitch OverridePitch { get; set; }
    public delegate void OverrideCameraYaw(ref CameraParams Cam, ref float InOutYaw);
    public OverrideCameraYaw OverrideYaw { get; set; }
    public delegate void OverrideCameraDist(ref CameraParams Cam, ref float InOutDist);
    public OverrideCameraDist OverrideDist { get; set; }
    
    private void Awake() {
        _currentSettings = initialSettings;

        // if( save data exists )
        // Init From Save File(saveData)
        // else
        Init();
    }
    
    // Set default new save settings here
    private void Init() {
        _distanceZone = 1;
        _cameraParams = new CameraParams();
        _cameraParams.pitch = 30f;
        _cameraParams.yaw = _transform.eulerAngles.y;
        _cameraParams.distance = DesiredDistance(_distanceZone);
    }
    
    private void Start() {
        if (CameraManager.Exists) {
            CameraManager.Instance.AddCamera(this);
        }
    }

    public void Activate(PreviousCameraInfo info) {
        if (info.PlayerCameraShouldMatch) {
            _cameraParams.pitch = info.Rotation.eulerAngles.x;
            _cameraParams.yaw = info.Rotation.eulerAngles.y;
        }
    }
    
    public void Deactivate() {}

    // Variables for storing this frame's input.
    private float _pitchInput;
    private float _yawInput;
    private int _distanceInput;
    private bool _canDistanceZoom = true;
    private void UpdateInput(Vector2 look, int distanceDelta) {
        _yawInput = look.x;
        _pitchInput = look.y;
        _distanceInput = distanceDelta;
        // if mouse scroll clamp, if gamepad button press then loop
        if (_distanceInput != 0 && _canDistanceZoom) {
            _distanceZone = Mathf.Clamp(_distanceZone + _distanceInput, 0, 2);
            _canDistanceZoom = false;
        } else if (distanceDelta == 0) {
            _canDistanceZoom = true;
        }
    }

    public void UpdateCamera() {
        UpdateInput(PlayerInput.Instance.Look, PlayerInput.Instance.CameraDistanceToggle);
        
        float desiredPitch = DesiredPitch(_pitchInput);
        float desiredYaw = DesiredYaw(_yawInput);
        float desiredDistance = DesiredDistance(_distanceZone);

        Vector3 trackingPoint = DesiredTrackingPoint();
        
        Vector2 framing = new Vector2(initialSettings.XFraming, initialSettings.YFraming);

        if (OverridePitch != null) {
            OverridePitch(ref _cameraParams, ref desiredPitch);
        }
        
        if (OverrideYaw != null) {
            OverrideYaw(ref _cameraParams, ref desiredYaw);
        }
        
        ApplyCameraValues(trackingPoint, desiredPitch, desiredYaw, desiredDistance, framing);
    }

    public int Priority { get; set; }

    private Vector3 DesiredTrackingPoint() {
        if (movement.Grounded) {
            
        }
        return _transform.position + Vector3.up * _currentSettings.TrackingHeight;
    }


    private bool _previouslyWasHidden = false;
    private Collider _currentObscuringCollider;
    
    private void ApplyCameraValues(Vector3 trackingPoint, float desiredPitch, float desiredYaw, float desiredDistance, Vector2 framing) {
        _cameraParams.trackingPoint = trackingPoint;
        _cameraParams.pitch = desiredPitch;
        _cameraParams.yaw = desiredYaw;
        _cameraParams.framing = framing;

        Location intendedLocation = CameraUtils.LocationFromParams(FieldOfView(), in _cameraParams, CameraManager.Instance.GameCamera.aspect);
        Vector3 rayForward = (intendedLocation.position - trackingPoint).normalized;
        
        
        if (CantSeeAvatar(trackingPoint, rayForward, desiredDistance, out RaycastHit hit)) {
            desiredDistance = hit.distance;
            
            // If we previously could see the avatar then we know we are about to crash zoom and break the 30 degree rule.
            // To lessen the impact a little, we set the current distance of the camera to the exact hit point.
            // Then as the decay gets applied, the camera will iterate towards the ideal distance without passing through the object. 
            if (!_previouslyWasHidden ||  hit.collider != _currentObscuringCollider) {
                _previouslyWasHidden = true;
                _currentObscuringCollider = hit.collider;
                
                _cameraParams.distance = Mathf.Min(desiredDistance + _currentSettings.SphereCastRadius, _cameraParams.distance);
            }
        } else {
            _previouslyWasHidden = false;
            _currentObscuringCollider = hit.collider;
        }
        
        _cameraParams.distance = _cameraParams.distance.ExponentialDecay(desiredDistance, generalConfig.DistanceDecay, Time.deltaTime);
        
        
        _location = CameraUtils.LocationFromParams(FieldOfView(), in _cameraParams,  CameraManager.Instance.GameCamera.aspect);
    }

    private bool CantSeeAvatar(Vector3 trackingPoint, Vector3 rayForward, float desiredDistance, out RaycastHit hit) {
        Ray ray = new Ray(trackingPoint, rayForward);
        if (Physics.SphereCast(ray, initialSettings.SphereCastRadius, out hit, desiredDistance, cameraObstacleMask, QueryTriggerInteraction.Ignore)) {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected() {
        _cameraParams.trackingPoint = transform.position + Vector3.up * initialSettings.TrackingHeight;
    }

    private float DesiredPitch(float pitchDelta) {
        float pitch = pitchDelta * generalConfig.CameraSpeed + _cameraParams.pitch;
        pitch = Mathf.Clamp(pitch, generalConfig.MinPitch,generalConfig.MaxPitch);
        return pitch;
    }

    private float DesiredYaw(float yawDelta) {
        float yaw = yawDelta * generalConfig.CameraSpeed + _cameraParams.yaw;
        yaw = MathUtils.Wrap(yaw, 0f, 360f);
        return yaw;
    }
    
    public float DesiredDistance(int distanceZone) {
        switch (distanceZone) {
            case 0: return _currentSettings.MinDistance;
            case 1: return _currentSettings.MidDistance;
            case 2: return _currentSettings.MaxDistance;
        }

        Debug.LogError(distanceZone + "is not a valid distance zone");
        return 20f;
    }
}
