using CameraSystem;
using UnityEngine;

public class ArtRootRotator : MonoBehaviour
{
    [SerializeField] private Transform _artRoot;
    [SerializeField] private Rigidbody _playerRigidBody;
    private Vector3 _previousPos;
    
    private void Start() {
        CameraManager.Instance.OnCameraUpdateFinished += AfterCameraUpdate;
    }

    private void OnDestroy()
    {
        if (CameraManager.Exists)
        {
            CameraManager.Instance.OnCameraUpdateFinished -= AfterCameraUpdate;
        }
    }

    private void AfterCameraUpdate() {
        if (PlayerInput.Exists)
        {
            Vector2 move = PlayerInput.Instance.Move;
            if (move.sqrMagnitude > 0.001f)
            {
                FaceInputDirection(move);
            }
            else
            {
                FaceVelocityDirection();
            }
        }
        else
        {
            FaceVelocityDirection();
        }
    }

    private void FaceInputDirection(Vector2 move)
    {
        Vector2 v = move.normalized;
        float angle = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
        angle = angle < 0 ? angle + 360 : angle;
        _artRoot.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void FaceVelocityDirection()
    {
        Vector3 v = _previousPos - _playerRigidBody.position;
        _previousPos = _playerRigidBody.position;
        if (v.magnitude > 0.1f) {
            v = v.normalized;
            float angle = Mathf.Atan2(-v.x, -v.z) * Mathf.Rad2Deg;
            angle = angle < 0 ? angle + 360 : angle;

            _artRoot.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
}
