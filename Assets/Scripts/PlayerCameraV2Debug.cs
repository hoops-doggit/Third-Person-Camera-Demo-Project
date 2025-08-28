using ThisNamespace;
using UnityEngine;

public class PlayerCameraV2Debug : MonoBehaviour {
    [SerializeField] private OrbitCamera cam;
    [SerializeField] private OrbitCameraGeneralConfig general;
    [SerializeField] private OrbitCameraSettingsConfig settings;
    [SerializeField] private int distanceArcDivisions = 20;
    private void OnDrawGizmos() {
        CameraParams param = cam.CameraParams;
        Vector3 trackingPoint = param.trackingPoint;
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
        
        float distance0 = DesiredDistance(0);
        float distance1 = DesiredDistance(1);
        float distance2 = DesiredDistance(2);
        
        Vector3 a;
        for (int i = 1; i <= distanceArcDivisions; i++) {
            float pitcha = Mathf.Lerp(general.MinPitch, general.MaxPitch, (float)(i-1) / distanceArcDivisions);
            float pitchb = Mathf.Lerp(general.MinPitch, general.MaxPitch, (float)i / distanceArcDivisions);
            float aMult = settings.DistanceXPitchCurve.Evaluate(pitcha);
            float bMult = settings.DistanceXPitchCurve.Evaluate(pitchb);
            a = Quaternion.AngleAxis(pitcha, Vector3.right) * (Vector3.back * (distance0 * aMult));
            a += trackingPoint;
            b = Quaternion.AngleAxis(pitchb, Vector3.right) * (Vector3.back * (distance0 * bMult));
            b += trackingPoint;
            Gizmos.DrawLine(a, b);
            
            a = Quaternion.AngleAxis(pitcha, Vector3.right) * (Vector3.back * (distance1 * aMult));
            a += trackingPoint;
            b = Quaternion.AngleAxis(pitchb, Vector3.right) * (Vector3.back * (distance1 * bMult));
            b += trackingPoint;
            Gizmos.DrawLine(a, b);
            
            a = Quaternion.AngleAxis(pitcha, Vector3.right) * (Vector3.back * (distance2 * aMult));
            a += trackingPoint;
            b = Quaternion.AngleAxis(pitchb, Vector3.right) * (Vector3.back * (distance2 * bMult));
            b += trackingPoint;
            Gizmos.DrawLine(a, b);
        }
    }

    private float DesiredDistance(int zone) {
        switch (zone) {
            case 0: return settings.MinDistance;
            case 1: return settings.MidDistance;
            case 2: return settings.MaxDistance;
        }

        return 0;
    }
}
