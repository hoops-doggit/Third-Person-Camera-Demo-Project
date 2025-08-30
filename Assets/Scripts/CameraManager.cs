using System.Collections.Generic;
using ThisNamespace;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera gameCamera = null;
    [SerializeField] private TransitionConfig transitionConfig = null;
    private Transform _gameCameraTransform = null;
    private IVirtualCamera _activeCamera;
    
    private readonly HashSet<IVirtualCamera> _activeCameras = new HashSet<IVirtualCamera>();
    private PreviousCameraInfo _previousCameraInfo = new();
    
    public Camera GameCamera => gameCamera;

    protected override void Awake() {
        base.Awake();
        _gameCameraTransform = gameCamera.transform;
    }

    public void AddCamera(IVirtualCamera virtualCamera) {
        if (virtualCamera == null) {
            return;
        }
        _activeCameras.Add(virtualCamera);
    }

    public void RemoveCamera(IVirtualCamera virtualCamera) {
        if (_activeCamera == virtualCamera) {
            _previousCameraInfo = new(_activeCamera);
            _activeCamera.Deactivate();
            _activeCameras.Remove(virtualCamera);
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

    private bool _blending;
    private float _blendStartTime;
    private float _blendDuration;
    
    private void LateUpdate() {
        IVirtualCamera currentHighestPriorityCamera = GetHighestPriorityCamera();
        if (_activeCamera != currentHighestPriorityCamera) {
            if (_activeCamera != null) {
                IVirtualCamera previousCamera = _activeCamera;
                _activeCamera = null;
                _activeCameras.Remove(previousCamera);
                _previousCameraInfo = new(previousCamera);
                previousCamera.Deactivate();
            }
            _activeCamera = currentHighestPriorityCamera;
            _activeCamera.Activate(_previousCameraInfo);
            
            _blendDuration = -1;
            if (transitionConfig.GetTransition(_previousCameraInfo.Name, _activeCamera.Name, out TransitionInfo info)) {
                if (info.Duration > 0) {
                    _blendDuration = info.Duration;
                    _blending = true;
                    _blendStartTime = Time.time;
                }
            }
        }
        
        if (_activeCamera == null) {
            return;
        }
        
        _activeCamera.UpdateCamera();

        Vector3 position;
        Quaternion rotation;
        float fieldOfView;
        
        if (_blending) {
            float t = MathUtils.SafeDivide(Time.time - _blendStartTime, _blendDuration);
            position = Vector3.Lerp(_previousCameraInfo.Position, _activeCamera.Position(), t);
            rotation = Quaternion.Slerp(_previousCameraInfo.Rotation, _activeCamera.Rotation(), t);
            fieldOfView = Mathf.Lerp(_previousCameraInfo.FieldOfView, _activeCamera.FieldOfView(), t);
        } else {
            position = _activeCamera.Position();
            rotation = _activeCamera.Rotation();
            fieldOfView = _activeCamera.FieldOfView();
        }
        
        _gameCameraTransform.SetPositionAndRotation(position, rotation);
        gameCamera.fieldOfView = fieldOfView;
    }
}
