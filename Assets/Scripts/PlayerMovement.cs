using ThisNamespace;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float moveSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float groundedCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundCheckLayer;

    private bool _grounded = true;
    public bool Grounded => _grounded;
    private Vector3 _groundPosition;
    
    private PlayerInput _playerInput;
    private Vector3 _velocity;
    public Vector3 GroundPosition => _groundPosition;
    public Vector3 Velocity => _velocity;
    
    private Transform _t;

    private void Awake() {
        _t = transform;
    }

    private void Start() {
        _playerInput = PlayerInput.Instance;
    }

    private void Update() {
        if (_playerInput.Move.sqrMagnitude > 0.0001f) {
            Vector3 inputDir = new Vector3(_playerInput.Move.x, 0f, _playerInput.Move.y).normalized;
            Vector3 movementDir = inputDir;
            if (_velocity.sqrMagnitude > 0.1f ){
                Vector3 currentDir = _velocity.normalized;
                float inputDotMovement = Vector3.Dot(movementDir, currentDir);
                if(inputDotMovement > -0.5){
                    movementDir = Vector3.RotateTowards(currentDir, inputDir, rotationSpeed * Time.deltaTime, 0f);
                }
            }
            _velocity = movementDir * moveSpeed;
        } else {
            if (_velocity.magnitude < 0.01f) {
                _velocity = Vector3.zero;
            } else {
                float mag = _velocity.magnitude;
                Vector3 dir = _velocity.normalized;
                mag = mag.ExponentialDecay(0, drag, Time.deltaTime);
                _velocity = dir * mag;
            }
        }
        rb.MovePosition(_t.position + _velocity * Time.deltaTime);

        if (Physics.SphereCast(_t.position, groundedCheckRadius, Vector3.down, out RaycastHit hit, 20, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
            _groundPosition = hit.point;
            if (Vector3.Distance(hit.point, _t.position) < 0.01f) {
                _grounded = true;
            } else {
                _grounded = false;
            }
        }
    }
    

}
