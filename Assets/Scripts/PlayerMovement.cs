using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float moveSpeed;
    [SerializeField] private float drag;
    [SerializeField] private Rigidbody rb;
    
    private PlayerInput _playerInput;
    private Vector3 _velocity;
    
    private Transform _t;

    private void Awake() {
        _t = transform;
    }

    private void Start() {
        _playerInput = PlayerInput.Instance;
    }

    private void Update() {
        if (_playerInput.Move.sqrMagnitude > 0.0001f) {
            Vector3 dir = new Vector3(_playerInput.Move.x, 0f, _playerInput.Move.y).normalized;
            _velocity = dir * moveSpeed;
        } else {
            // float damping = Mathf.Exp(-drag * Time.fixedDeltaTime);
            // _velocity *= damping;
            // if (_velocity.magnitude < 0.01f) {
            //     _velocity = Vector3.zero;
            // }
            _velocity = Vector3.zero;
        }
        //rb.AddForce(_velocity, ForceMode.VelocityChange);
        rb.MovePosition(_t.position + _velocity * Time.deltaTime);
    }
}
