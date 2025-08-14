using System;
using DefaultNamespace;
using UnityEngine;

public class PlayerInput : Singleton<PlayerInput> {
    private Vector2 _move;
    private Vector2 _look;
    private int _cameraDistanceLevel;

    [SerializeField] private bool _invertDistanceLevel, _invertPitch;

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
        Vector2 cameraRelativeMovement = TransformInputToCameraRelative(rawMovement, CameraManager.Instance.GameCamera.transform);
        _move = cameraRelativeMovement;
        
        _look = _actions.Player.Look.ReadValue<Vector2>();
        _look.y = _invertPitch ? -_look.y : _look.y;
        float t = _actions.Player.IncrementCameraDistanceZone.ReadValue<float>();
        _cameraDistanceLevel = (int)t;
        _cameraDistanceLevel *= _invertDistanceLevel ? 1 : -1;
    }
    
    Vector2 TransformInputToCameraRelative(Vector2 input, Transform cam)
    {
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();
        
        return (input.x * right + input.y * forward).XZ();
    }
}
