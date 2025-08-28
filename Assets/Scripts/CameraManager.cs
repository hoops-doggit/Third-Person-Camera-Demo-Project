using System.Collections.Generic;
using ThisNamespace;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] Camera _gameCamera = null;
    private Transform _gameCameraTransform = null;
    private IVirtualCamera _activeCamera;
    
    HashSet<IVirtualCamera> _activeCameras = new HashSet<IVirtualCamera>();
    private PreviousCameraInfo _previousCameraInfo = null;
    
    public Camera GameCamera => _gameCamera;

    protected override void Awake() {
        base.Awake();
        _gameCameraTransform = _gameCamera.transform;
    }

    public void AddCamera(IVirtualCamera virtualCamera) {
        if (virtualCamera == null) {
            return;
        }
        _activeCameras.Add(virtualCamera);
    }

    public void RemoveCamera(IVirtualCamera virtualCamera) {
        if (_activeCamera == virtualCamera) {
            _activeCamera = null;
        }
    }

    private IVirtualCamera GetHighestPriorityCamera() {
        IVirtualCamera highestPriorityCamera = null;
        int highestPriorityCameraIndex = -1;
        foreach (IVirtualCamera camera in _activeCameras) {
            if (camera.Priority > highestPriorityCameraIndex) {
                highestPriorityCamera = camera;
                highestPriorityCameraIndex = camera.Priority;
            }
        }

        return highestPriorityCamera;
    }

    private void LateUpdate() {
        IVirtualCamera currentHighestPriorityCamera = GetHighestPriorityCamera();
        if (_activeCamera != currentHighestPriorityCamera) {
            if (_activeCamera != null) {
                _activeCamera.Deactivate();
            }
        }
        _activeCamera.UpdateCamera();
        
        _gameCameraTransform.SetPositionAndRotation(_activeCamera.Position(), _activeCamera.Rotation());
        _gameCamera.fieldOfView = _activeCamera.FieldOfView();
    }
}
