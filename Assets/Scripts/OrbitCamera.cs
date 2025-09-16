using System;
using SODynamics;
using ThisNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class OrbitCamera : MonoBehaviour, IVirtualCamera
{
    [SerializeField] private OrbitCameraGeneralConfig generalConfig;
    [SerializeField] private OrbitCameraSettingsConfig initialSettings;
    [SerializeField] private LayerMask cameraObstacleMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private PlayerMovement movement;
    [AngularSpringDrawer]
    [SerializeField] private AngularSpring yawSpring;
    
    private OrbitCameraSettingsConfig _currentSettings;
    [FormerlySerializedAs("_transform")] [SerializeField] private Transform playerTransform;
    private Location _location;
    private bool _Active;
    
    // * Persistant state
    private int _distanceZone = 1;   
    [SerializeField] // just so we can see it in editor
    private CameraParams _cameraParams;
    //

    // PointOFView
    public CameraParams CameraParams => _cameraParams;
    public string Name => "PlayerCamera";
    public bool Active => _Active;
    public Vector3 Position() => _location.position;
    public Quaternion Rotation() => _location.rotation;
    public float FieldOfView() => _currentSettings.FieldOfView * _currentSettings.FieldOfViewXPitchCurve.Evaluate(_cameraParams.pitch);
    
    
    public delegate void OverrideCameraTracking(ref CameraParams cp, ref Vector3 InOutTrackingPoint);
    public OverrideCameraTracking OverrideTracking { get; set; }
    public delegate void OverrideCameraFraming(ref CameraParams cp, ref Vector2 InOutFraming);
    public OverrideCameraFraming OverrideFraming { get; set; }
    public delegate void OverrideCameraPitch(ref CameraParams cp, ref float InOutPitch);
    public OverrideCameraPitch OverridePitch { get; set; }
    public delegate void OverrideCameraYaw(ref CameraParams cp, ref float InOutYaw);
    public OverrideCameraYaw OverrideYaw { get; set; }
    public delegate void OverrideCameraDist(ref CameraParams cp, ref float InOutDist);
    public OverrideCameraDist OverrideDist { get; set; }
    
    private void Awake() {
        _currentSettings = initialSettings;

        // if( save data exists )
        // Init From Save File(saveData)
        // else
        Init();
    }
    
    // Set default for brand new save here
    private void Init() {
        _distanceZone = 1;
        _cameraParams = new CameraParams();
        _cameraParams.pitch = 30f;
        _cameraParams.yaw = playerTransform.eulerAngles.y;
        _cameraParams.distance = DesiredDistance(_distanceZone, _cameraParams.pitch);
        _prevPos = playerTransform.position;
    }
    
    private void Start() {
        if (CameraManager.Exists) {
            CameraManager.Instance.AddCamera(this);
        }
    }

    public void Activate(PreviousCameraInfo info) {
        if (info != null && info.PlayerCameraShouldMatch) {
            _cameraParams.pitch = info.Rotation.eulerAngles.x;
            _cameraParams.yaw = info.Rotation.eulerAngles.y;
        }
    }

    public void Deactivate() {
        //
    }

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

        float inputThreshold = 0.01f;
        // Is camera input above threshold?
        if (look.SqrMagnitude() < inputThreshold * inputThreshold) {
            // Has input been consistently above threshold or was input bumped? 
            if (!_inputReleased) {
                _timeInputReleased = Time.time;
                _inputReleased = true;
            }
        } else {
            _inputReleased = false;
        }

    }

    private bool _inputReleased = false;
    private float _timeInputReleased = 0;
    
    public void UpdateCamera() {
        // For ease of use I'm calling this here, but there may be a case for you to sample input at the beginning 
        // of the frame, rather than in CameraManager.LateUpdate()
        UpdateInput(PlayerInput.Instance.Look, PlayerInput.Instance.CameraDistanceToggle);
        
        float desiredPitch = DesiredPitch(_pitchInput);
        float desiredYaw = DesiredYaw(_yawInput);

        
        bool cameraInputElapsed = false;
        if (_inputReleased) {
            cameraInputElapsed = Time.time - _timeInputReleased > generalConfig.GravityDelay;
        }

        if (/* Player has enabled Camera Auto Adjust setting */ true) {
            desiredYaw = YawGravity(movement.Velocity, PlayerInput.Instance.MoveRaw, desiredYaw, cameraInputElapsed);
        }
        
        //desiredPitch = PitchGravity(desiredPitch, cameraInputElapsed);
        
        float desiredDistance = DesiredDistance(_distanceZone, desiredPitch); 

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
    
    private float YawGravity(Vector3 movementVelocity, Vector2 rawMoveInput, float currentYaw, bool cameraInputElapsed) {
        if (cameraInputElapsed) {
            // We do want to rotate the camera towards the angle representing the direction of player movement
            float targetAngle = movementVelocity.YawDeg360();
            
            bool movingFastEnough = movementVelocity.magnitude > generalConfig.YawGravityVelocityThreshold;

            float normalizedAngle = Mathf.Atan2(rawMoveInput.y, rawMoveInput.x) * Mathf.Rad2Deg - 90;
            float inputAngle = normalizedAngle.Wrap(0,360);
            Debug.Log("input angle " + inputAngle);
            bool inputOutsideDeadZone = IsOutsideDeadZone(inputAngle);
            
            bool yawGravityEnabled = movingFastEnough && inputOutsideDeadZone;
            return yawSpring.Update(targetAngle, Time.deltaTime, yawGravityEnabled);
        }

        yawSpring.CurrentAngle = currentYaw;
        
        return yawSpring.Update(currentYaw, Time.deltaTime, true);
    }

    private bool IsOutsideDeadZone(float inputAngle) {
        if (inputAngle < generalConfig.YawGravityFrontDeadZone &&
            inputAngle > 360 - generalConfig.YawGravityFrontDeadZone) {
            return false;
        }

        if (inputAngle > generalConfig.YawGravityBackDeadZone &&
            inputAngle < 360 - generalConfig.YawGravityBackDeadZone) {
            return false;
        }

        return true;
    }


    private float YawGravity1(Vector3 movementVelocity, Vector2 rawMoveInput, float currentYaw, bool cameraInputElapsed) {
        bool movingFastEnough = movementVelocity.magnitude > generalConfig.YawGravityVelocityThreshold;
        
        // returns between -180 and 180 - Atan2 = Four Quadrant Arc Tangent
        float normalizedTarget = Mathf.Atan2(movementVelocity.x, movementVelocity.z) * Mathf.Rad2Deg;
        
        // brings back to within 0 to 360
        float targetAngle = normalizedTarget < 0 ? normalizedTarget + 360 : normalizedTarget;

        // Same again but for movement input
        float normalizedInputAngle = Mathf.Atan2(rawMoveInput.y, rawMoveInput.x) * Mathf.Rad2Deg;
        float inputAngle = normalizedInputAngle < 0 ? normalizedInputAngle + 360 : normalizedInputAngle;
        
        bool inputOutsideDeadZone = inputAngle > generalConfig.YawGravityFrontDeadZone && inputAngle < generalConfig.YawGravityBackDeadZone;
        
        // Should yaw gravity currently even be enabled? 
        // If we don't want it and it previously was moving then it should smoothly interpolate velocity to 0
        bool yawGravityEnabled = movingFastEnough && inputOutsideDeadZone;

        return 0;//next slide;
    }


    private float _f1,_z1,_r1, _pitchGravity;
    private SecondOrderDynamicsFloat _pitchSpring;
    private float PitchGravity(float currentPitch,  bool cameraInputElapsed) {
        float desiredPitch = currentPitch;

        if (_pitchSpring == null || Math.Abs(generalConfig.YawGravityFrequency + generalConfig.YawGravityDamping + generalConfig.YawGravityResponse - (_f1 + _z1 + _r1)) > 0.001f) {
            ResetPitchSpring();
        }

        Vector3 groundNormal = Vector3.up;
        if (Physics.Raycast(playerTransform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 5, groundMask, QueryTriggerInteraction.Ignore)) {
            groundNormal = hit.normal;
        }
        
        Vector3 groundNormalWithoutYaw = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
        
        float slope = Vector3.Angle(Vector3.up, hit.normal);
        // Determine the direction (clockwise or counterclockwise)
        Vector3 cross = Vector3.Cross(Vector3.up, groundNormalWithoutYaw);
        float sign = cross.z > 0 ? 1 : -1;

        float minPitch = _currentSettings.MinMaxPreferredPitch.x + slope * sign;
        float maxPitch = _currentSettings.MinMaxPreferredPitch.y + slope * sign;
        
        Debug.Log("Current pitch = " + desiredPitch + ", min: " + minPitch + ", max: " + maxPitch);

        if (cameraInputElapsed) {
            if (_cameraParams.pitch > maxPitch) {
                desiredPitch = _cameraParams.pitch.ExponentialDecay(maxPitch, generalConfig.PitchGravityStrength, Time.deltaTime);
            } else if (_cameraParams.pitch < minPitch) {
                desiredPitch = _cameraParams.pitch.ExponentialDecay(minPitch, generalConfig.PitchGravityStrength, Time.deltaTime);
            }
        }

        return desiredPitch;
    }

    private void ResetPitchSpring() {
        _f1 = generalConfig.PitchGravityFrequency;
        _z1 = generalConfig.PitchGravityDamping;
        _r1 = generalConfig.PitchGravityResponse;
        _pitchSpring = new SecondOrderDynamicsFloat(generalConfig.PitchGravityFrequency,generalConfig.PitchGravityDamping,generalConfig.PitchGravityResponse,_pitchGravity);
    }


    public int Priority { get; set; }
    private Vector3 _prevPos;
    public Vector3 DesiredTrackingPoint() {
        if (movement.Grounded) {
            // do jump stuff here
        }
        Vector3 curPos = playerTransform.position;
        Vector3 position = new(
            _prevPos.x.ExponentialDecay(curPos.x, 16, Time.deltaTime),
            _prevPos.y.ExponentialDecay(curPos.y + _currentSettings.TrackingHeight, 16, Time.deltaTime),
            _prevPos.z.ExponentialDecay(curPos.z, 16, Time.deltaTime)
        );
        _prevPos = position;
        return position;
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
        if (pitch > generalConfig.MaxPitch) {
            pitch = pitch.ExponentialDecay(generalConfig.MaxPitch, generalConfig.PitchGravityStrength, Time.deltaTime);
        } else if (pitch < generalConfig.MinPitch) {
            pitch = pitch.ExponentialDecay(generalConfig.MinPitch, generalConfig.PitchGravityStrength, Time.deltaTime);
        }
        return pitch;
    }

    private float DesiredYaw(float yawDelta) {
        float yaw = yawDelta * generalConfig.CameraSpeed + _cameraParams.yaw;
        yaw = MathUtils.Wrap(yaw, 0f, 360f);
        return yaw;
    }
    
    public float DesiredDistance(int distanceZone, float pitch) {
        float distance = 20;
        switch (distanceZone) {
            case 0: distance = _currentSettings.MinDistance; break;
            case 1: distance = _currentSettings.MidDistance; break;
            case 2: distance = _currentSettings.MaxDistance; break;
        }
        return distance * _currentSettings.DistanceXPitchCurve.Evaluate(pitch);;
    }
}
