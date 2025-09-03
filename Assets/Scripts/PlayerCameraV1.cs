using ThisNamespace;
using UnityEngine;

public class PlayerCameraV1 : MonoBehaviour, IVirtualCamera {
    [SerializeField] private OrbitCameraGeneralConfig generalConfig;
    [SerializeField] private OrbitCameraSettingsConfig initialSettings;
    [SerializeField] private new Transform transform;
    [SerializeField] private LayerMask obstacleLayerMask;

    private OrbitCameraSettingsConfig _currentSettings;
    private PlayerCameraState _state;

    private Vector3 _desiredPosition;
    private Quaternion _desiredRotation;
    private float _desiredFov;
    private Vector3 _inputThisFrame;
    private bool _canDistanceZoom;

    public string Name { get; }
    public Vector3 Position() => _desiredPosition;
    public Quaternion Rotation() => _desiredRotation;
    public float FieldOfView() => _currentSettings.FieldOfView * _currentSettings.FieldOfViewXPitchCurve.Evaluate(_desiredRotation.eulerAngles.x);

    private void Awake() {
        transform = GetComponent<Transform>();
        _state = new PlayerCameraState(initialSettings);
        _currentSettings = initialSettings;
    }

    // Turn this into a public method which gets called once all game loading has finished
    private void Start() {
        if (CameraManager.Exists) {
            CameraManager.Instance.AddCamera(this);
        }
    }

    private void Update() {
        UpdateInput(PlayerInput.Instance.Look, PlayerInput.Instance.CameraDistanceToggle);
        UpdateCamera();
    }

    /// <summary>
    /// After we've inverted input, smoothed gamepad input etc. we pass it to the camera
    /// </summary>
    public void UpdateInput(Vector2 pitchYawInput, int distanceInput) {
        _inputThisFrame = new Vector3(x: pitchYawInput.y, y: pitchYawInput.x, z: distanceInput);
        // if mouse scroll clamp, if gamepad button press then loop
        if (distanceInput != 0 && _canDistanceZoom) {
            _state.DistanceZone = Mathf.Clamp(_state.DistanceZone + distanceInput, 0, 2);
            _canDistanceZoom = false;
        } else if (distanceInput == 0) {
            _canDistanceZoom = true;
        }
    }



    /// <summary>
    /// This could be called by the camera manager since we will use that to store a reference to all of our virtual cameras
    /// </summary>
    public void UpdateCamera() {
        Vector2 pitchYaw = CalculatePitchYaw(_inputThisFrame.x, _inputThisFrame.y);

        float desiredDistance = DesiredDistance(_state.DistanceZone);
        desiredDistance *= _currentSettings.DistanceXPitchCurve.Evaluate(pitchYaw.x);

        Vector3 trackingPoint = transform.position + Vector3.up * initialSettings.TrackingHeight;

        Quaternion desiredRotation = Quaternion.Euler(pitchYaw.x, pitchYaw.y, 0f);

        if (CantSeeAvatar(trackingPoint, desiredRotation * Vector3.back, desiredDistance, out RaycastHit hit)) {
            desiredDistance = hit.distance;
        }

        ApplyCameraValues(trackingPoint, desiredRotation, desiredDistance, pitchYaw);
    }

    public int Priority { get; set; }
    public void Activate(PreviousCameraInfo info) {
        //
    }

    public void Deactivate() {
        //
    }

    private bool _Active;
    public bool Active => _Active;

    public void Disable() {
        //
    }

    private bool CantSeeAvatar(Vector3 trackingPoint, Vector3 rayForward, float desiredDistance, out RaycastHit hit) {
        Ray ray = new Ray(trackingPoint, rayForward);
        return Physics.SphereCast(ray, _currentSettings.SphereCastRadius, out hit, desiredDistance, obstacleLayerMask, QueryTriggerInteraction.Ignore);
    }

    private void ApplyCameraValues(Vector3 trackingPoint, Quaternion desiredRotation, float distance, Vector2 pitchYaw) {
        _state.CachePitchYaw(pitchYaw);

        Vector3 desiredPosition = Vector3.back * distance;

        desiredPosition = desiredRotation * desiredPosition;

        desiredPosition += trackingPoint;

        _desiredPosition = desiredPosition;
        _desiredRotation = desiredRotation;
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

    private Vector2 CalculatePitchYaw(float pitchDelta, float yawDelta) {
        float pitch = pitchDelta * generalConfig.CameraSpeed + _state.Pitch;
        pitch = Mathf.Clamp(pitch, generalConfig.MinPitch,generalConfig.MaxPitch);
        // clamp pitch
        float yaw = yawDelta * generalConfig.CameraSpeed + _state.Yaw;
        yaw = MathUtils.Wrap(yaw, 0f, 360f);
        // wrap yaw
        return new Vector2(x: pitch, y: yaw);
    }

    public class PlayerCameraState {
        //public float Distance;
        public float Pitch;
        public float Yaw;
        public int DistanceZone;

        public PlayerCameraState(float distance, float pitch, float yaw, int distanceZone) {
            //Distance = distance;
            Pitch = pitch;
            Yaw = yaw;
            DistanceZone = distanceZone;
        }
        
        public PlayerCameraState(OrbitCameraSettingsConfig settings) {
            //Distance = settings.MidDistance;
            DistanceZone = 1;
            Pitch = 0;
            Yaw = 0;
        }
        
        public PlayerCameraState() { }

        public void CachePitchYaw(Vector2 pitchYaw) {
            Pitch = pitchYaw.x;
            Yaw = pitchYaw.y;
        }
    }
}
