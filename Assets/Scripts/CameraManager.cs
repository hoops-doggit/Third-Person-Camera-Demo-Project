using DefaultNamespace;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] Camera _gameCamera = null;
    private Transform _gameCameraTransform = null;
    private IVirtualCamera _virtualCamera;
    bool _haveCamera = false;
    
    public Camera GameCamera => _gameCamera;

    protected override void Awake() {
        base.Awake();
        _gameCameraTransform = _gameCamera.transform;
    }

    public void AddCamera(IVirtualCamera virtualCamera) {
        if (virtualCamera != null) {
            _virtualCamera = virtualCamera;
            _haveCamera = true;
        }
    }

    public void RemoveCamera(IVirtualCamera virtualCamera) {
        if (_virtualCamera == virtualCamera) {
            _virtualCamera = null;
            _haveCamera = false;
        }
    }

    private void LateUpdate() {
        if (!_haveCamera) {
            return;
        }
        _virtualCamera.UpdateCamera();
        _gameCameraTransform.SetPositionAndRotation(_virtualCamera.Position(), _virtualCamera.Rotation());
        _gameCamera.fieldOfView = _virtualCamera.FieldOfView();
    }
}
