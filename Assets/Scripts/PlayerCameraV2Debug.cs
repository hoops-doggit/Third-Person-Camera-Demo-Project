using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCameraV2Debug : MonoBehaviour {
    [SerializeField] private ThirdPersonCamera cam;
    [SerializeField] private ThirdPersonGeneralConfig general;
    [SerializeField] private ThirdPersonSettingConfig settings;

    private void OnDrawGizmos() {
        CameraParams param = cam.CameraParams;
        Vector3 trackingPoint = cam.CameraParams.trackingPoint;
        float radius = settings.SphereCastRadius;
        Vector3 l = trackingPoint + Vector3.left * radius;
        Vector3 r = trackingPoint + Vector3.right * radius;
        Vector3 u = trackingPoint + Vector3.up * radius;
        Vector3 d = trackingPoint + Vector3.down * radius;
        Vector3 f = trackingPoint + Vector3.forward * radius;
        Vector3 b = trackingPoint + Vector3.back * radius;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(l,r);
        Gizmos.DrawLine(u,d);
        Gizmos.DrawLine(f,b);
        Gizmos.DrawWireSphere(trackingPoint, radius);
    }
}
