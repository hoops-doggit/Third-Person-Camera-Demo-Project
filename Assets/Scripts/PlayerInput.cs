using ThisNamespace;
using UnityEngine;

public class PlayerInput : Singleton<PlayerInput> {
    private Vector2 _move;
    private Vector2 _look;
    private int _cameraDistanceLevel;
    
    [SerializeField] private bool _invertDistanceLevel, _invertPitch, _invertYaw;
    [SerializeField] private AnimationCurve _lookInputResponseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private ControlMode _controlMode;
    
    public Vector2 Move => _move;
    public Vector2 Look => _look;
    public int CameraDistanceToggle => _cameraDistanceLevel;
    private InputSystem_Actions _actions;
    
    
    protected override void Awake() {
        base.Awake();
        _actions = new InputSystem_Actions();
        _actions.Enable();
    }

    void Update() {
        Vector2 rawMovement = _actions.Player.Move.ReadValue<Vector2>();
        _move = TransformInputToCameraRelative(rawMovement, CameraManager.Instance.GameCamera.transform);
        
        Vector2 rawLook = _actions.Player.Look.ReadValue<Vector2>();
        rawLook.x = _invertYaw ? -rawLook.x : rawLook.x;
        rawLook.y = _invertPitch ? -rawLook.y : rawLook.y;

        if (_controlMode == ControlMode.GamePad) {
            _look.x = _lookInputResponseCurve.Evaluate(rawLook.x);
            _look.y = _lookInputResponseCurve.Evaluate(rawLook.y);
        } else {
            // Add whatever mouse processing is required here. Maybe filtering noisy signal?
            _look = rawLook;
        }
        
        float t = _actions.Player.IncrementCameraDistanceZone.ReadValue<float>();
        _cameraDistanceLevel = (int)t;
        _cameraDistanceLevel *= _invertDistanceLevel ? 1 : -1;
    }
    
    // This is where we transform our input so that holding left results in world space direction 
    // that matches the camera left direction.
    Vector2 TransformInputToCameraRelative(Vector2 input, Transform cam) {
        Vector3 v = cam.rotation * new Vector3(input.x, 0f, input.y);
        return new Vector2(v.x, v.z);
    }
}
