using System;
using ThisNamespace;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float moveSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float groundedCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundCheckLayer;
    [SerializeField] private Transform artRoot;

    private bool _grounded = true;
    public bool Grounded => _grounded;
    private Vector3 _groundPosition;
    
    private PlayerInput _playerInput;
    private Vector3 _velocity;
    public Vector3 GroundPosition => _groundPosition;
    public Vector3 Velocity => _velocity;
    
    private Transform _t;

    Vector3 previousPos = Vector3.zero;
    private void Awake() {
        _t = transform;
        previousPos = rb.position;
    }

    private void Start() {
        _playerInput = PlayerInput.Instance;
    }

    private void Update() {
        Vector3 v = previousPos - rb.position;
        previousPos = rb.position;
        if (v.magnitude > 0.1f) {
            v = v.normalized;
            float angle = Mathf.Atan2(-v.x, -v.z) * Mathf.Rad2Deg;
            angle = angle < 0 ? angle + 360 : angle;

            artRoot.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void FixedUpdate() {
        if (_playerInput.Move.magnitude > 0.01f) {
            Vector3 inputDir = new Vector3(_playerInput.Move.x, 0f, _playerInput.Move.y).normalized;
            _velocity = inputDir * moveSpeed;
        } else {
            if (_velocity.magnitude < 0.01f) {
                _velocity = Vector3.zero;
            } else {
                float mag = _velocity.magnitude;
                Vector3 dir = _velocity.normalized;
                mag = ExponentialDecay(mag, 0, drag, Time.fixedDeltaTime);
                _velocity = dir * mag;
            }
        }

        if (rb != null) {
            rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime);
        }

        if (Physics.SphereCast(rb.position, groundedCheckRadius, Vector3.down, out RaycastHit hit, 20, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
            _groundPosition = hit.point;
            if (Vector3.Distance(hit.point, rb.position) < 0.01f) {
                _grounded = true;
            } else {
                _grounded = false;
            }
        }
    }
    
    
    public static float ExponentialDecay(float current, float target, float decayRate, float deltaTime)
    {
        // Ensure decayRate is non-negative
        if (decayRate < 0f)
            decayRate = 0f;

        // Compute exponential interpolation factor
        float t = 1f - Mathf.Exp(-decayRate * deltaTime);

        // Interpolate toward target
        float result = Mathf.Lerp(current, target, t);

        // Snap to target if close enough
        if (Mathf.Abs(result - target) < 0.0001f)
            result = target;

        return result;
    }

}
