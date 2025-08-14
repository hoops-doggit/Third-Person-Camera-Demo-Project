using DefaultNamespace;
using UnityEngine;

public class PlayerCameraV2 : MonoBehaviour, IVirtualCamera
{
    [SerializeField] private PlayerCameraGeneralSettings generalSettings;
    [SerializeField] private PlayerCameraSettings initialSettings;
    [SerializeField] private LayerMask cameraObstacleMask;
    
    private PlayerCameraSettings _currentSettings;
    private CameraParams _cameraParams;
    public CameraParams CameraParams => _cameraParams;
    private Transform _transform;
    private Location _location;
    private float _fov;
    
    // Variables for storing this frame's input.
    private float _pitchInput, _yawInput;
    private bool _canDistanceZoom = true;
    private int _distanceZone = 1, _distanceInput;    
    
    public Vector3 Position() => _location.position;
    public Quaternion Rotation() => _location.rotation;
    public float FieldOfView() => initialSettings.FieldOfView;
    
    private void Awake() {
        _transform = GetComponent<Transform>();
        _currentSettings = initialSettings;
    }
    
    private void Start() {
        if (CameraManager.Exists) {
            CameraManager.Instance.AddCamera(this);
        }
    }

    private void UpdateInput(Vector2 look, int distanceDelta) {
        _pitchInput = look.y;
        _yawInput = look.x;
        _distanceInput = distanceDelta;
        // if mouse scroll clamp, if gamepad button press then loop
        if (_distanceInput != 0 && _canDistanceZoom) {
            _distanceZone = Mathf.Clamp(_distanceZone + _distanceInput, 0, 2);
            _canDistanceZoom = false;
        } else if (distanceDelta == 0) {
            _canDistanceZoom = true;
        }
    }

    private Vector3 _smoothedTrackingPointVelocity;
    private float _smoothedDistanceVelocity;

    public void UpdateCamera() {
        UpdateInput(PlayerInput.Instance.Look, PlayerInput.Instance.CameraDistanceToggle);
        
        float desiredPitch = DesiredPitch(_pitchInput);
        float desiredYaw = DesiredYaw(_yawInput);
        float desiredDistance = DesiredDistance(_distanceZone);
        desiredDistance = Mathf.SmoothDamp(_cameraParams.distance, desiredDistance, ref _smoothedDistanceVelocity, 0.05f);

        Vector3 trackingPoint = _transform.position + Vector3.up * _currentSettings.TrackingHeight;
        
        Vector2 framing = new Vector2(initialSettings.XFraming, initialSettings.YFraming);
        Quaternion desiredRotation = Quaternion.Euler(desiredPitch, desiredYaw, 0);
        
        if (CantSeeAvatar(trackingPoint, desiredRotation * Vector3.back, desiredDistance, out RaycastHit hit)) {
            desiredDistance = hit.distance;
        }
        
        ApplyCameraValues(trackingPoint, desiredPitch, desiredYaw, desiredDistance, framing);
    }

    private void ApplyCameraValues(Vector3 trackingPoint, float desiredPitch, float desiredYaw, float desiredDistance, Vector2 framing) {
        _cameraParams.trackingPoint = trackingPoint;
        _cameraParams.pitch = desiredPitch;
        _cameraParams.yaw = desiredYaw;
        _cameraParams.distance = desiredDistance;
        _cameraParams.framing = framing;
        _location = CameraUtils.SetParams(FieldOfView(), in _cameraParams,  CameraManager.Instance.GameCamera.aspect);
    }

    private bool CantSeeAvatar(Vector3 trackingPoint, Vector3 rayForward, float desiredDistance, out RaycastHit hit) {
        Ray ray = new Ray(trackingPoint, rayForward);
        if (Physics.SphereCast(ray, initialSettings.SphereCastRadius, out hit, desiredDistance, cameraObstacleMask, QueryTriggerInteraction.Ignore)) {
            return true;
        }
        return false;
    }

    private float DesiredPitch(float pitchDelta) {
        float pitch = pitchDelta * generalSettings.CameraSpeed + _cameraParams.pitch;
        pitch = Mathf.Clamp(pitch, generalSettings.MinPitch,generalSettings.MaxPitch);
        return pitch;
    }

    private float DesiredYaw(float yawDelta) {
        float yaw = yawDelta * generalSettings.CameraSpeed + _cameraParams.yaw;
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
