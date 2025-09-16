using ThisNamespace;
using UnityEngine;

public class PlayerCameraV2Debug : MonoBehaviour {
    [SerializeField] private OrbitCamera cam;
    [SerializeField] private OrbitCameraGeneralConfig general;
    [SerializeField] private OrbitCameraSettingsConfig settings;
    [SerializeField] private int distanceArcDivisions = 20;
    [SerializeField] private Transform playerRoot;
    private void OnDrawGizmos() {
        CameraParams param = cam.CameraParams;
        Matrix4x4 m = Matrix4x4.TRS(playerRoot.position, Quaternion.AngleAxis(param.yaw, Vector3.up), Vector3.one);
        Vector3 trackingPoint = Vector3.up * settings.TrackingHeight;
        float radius = settings.SphereCastRadius;

        Vector3 l = m.MultiplyPoint(trackingPoint + Vector3.left * radius);
        Vector3 r = m.MultiplyPoint(trackingPoint + Vector3.right * radius);
        Vector3 u = m.MultiplyPoint(trackingPoint + Vector3.up * radius);
        Vector3 d = m.MultiplyPoint(trackingPoint + Vector3.down * radius);
        Vector3 f = m.MultiplyPoint(trackingPoint + Vector3.forward * radius);
        Vector3 b = m.MultiplyPoint(trackingPoint + Vector3.back * radius);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(l, r);
        Gizmos.DrawLine(u, d);
        Gizmos.DrawLine(f, b);
        
        Vector3 worldSpaceTrackingPoint = m.MultiplyPoint(trackingPoint);
        Gizmos.DrawWireSphere(worldSpaceTrackingPoint, radius);
        
        Gizmos.DrawLine(m.MultiplyPoint(Vector3.zero), worldSpaceTrackingPoint);
        DrawString("Tracking Point", worldSpaceTrackingPoint);

        Gizmos.color = new Color(1,.5f,.5f,1);
        float maxPitchDistance = DesiredDistance(2) * settings.DistanceXPitchCurve.Evaluate(general.MaxPitch);
        Vector3 maxPitchPosition = Quaternion.AngleAxis(general.MaxPitch, Vector3.right) * (Vector3.back * maxPitchDistance);
        maxPitchPosition += trackingPoint;
        maxPitchPosition = m.MultiplyPoint(maxPitchPosition);
        Gizmos.DrawLine(worldSpaceTrackingPoint, maxPitchPosition);
        DrawString("maxPitch", maxPitchPosition);

        float minPitchDistance = DesiredDistance(2) * settings.DistanceXPitchCurve.Evaluate(general.MinPitch);
        Vector3 minPitchPosition = Quaternion.AngleAxis(general.MinPitch, Vector3.right) * (Vector3.back * minPitchDistance);
        minPitchPosition += trackingPoint;
        minPitchPosition = m.MultiplyPoint(minPitchPosition);
        Gizmos.DrawLine(worldSpaceTrackingPoint, minPitchPosition);
        DrawString("minPitch", minPitchPosition);
        
        
        Gizmos.color = new Color(.5f,.2f,.8f,1);
        float pitchDistance = DesiredDistance(2) * settings.DistanceXPitchCurve.Evaluate(settings.MinMaxPreferredPitch.y);
        Vector3 prefferredPos = Quaternion.AngleAxis(settings.MinMaxPreferredPitch.y, Vector3.right) * (Vector3.back * pitchDistance);
        prefferredPos += trackingPoint;
        prefferredPos = m.MultiplyPoint(prefferredPos);
        Gizmos.DrawLine(worldSpaceTrackingPoint, prefferredPos);
        DrawString("Preferred Max Pitch", prefferredPos);

        pitchDistance = DesiredDistance(2) * settings.DistanceXPitchCurve.Evaluate(settings.MinMaxPreferredPitch.x);
        prefferredPos = Quaternion.AngleAxis(settings.MinMaxPreferredPitch.x, Vector3.right) * (Vector3.back * pitchDistance);
        prefferredPos += trackingPoint;
        prefferredPos = m.MultiplyPoint(prefferredPos);
        Gizmos.DrawLine(worldSpaceTrackingPoint, prefferredPos);
        DrawString("Preferred Min Pitch", prefferredPos);
        
        
        Gizmos.color = Color.cyan;
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
            Gizmos.DrawLine(m.MultiplyPoint(a), m.MultiplyPoint(b));
            
            a = Quaternion.AngleAxis(pitcha, Vector3.right) * (Vector3.back * (distance1 * aMult));
            a += trackingPoint;
            b = Quaternion.AngleAxis(pitchb, Vector3.right) * (Vector3.back * (distance1 * bMult));
            b += trackingPoint;
            Gizmos.DrawLine(m.MultiplyPoint(a), m.MultiplyPoint(b));
            
            a = Quaternion.AngleAxis(pitcha, Vector3.right) * (Vector3.back * (distance2 * aMult));
            a += trackingPoint;
            b = Quaternion.AngleAxis(pitchb, Vector3.right) * (Vector3.back * (distance2 * bMult));
            b += trackingPoint;
            Gizmos.DrawLine(m.MultiplyPoint(a), m.MultiplyPoint(b));
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
    
    private GUIStyle TextStyle;
    private void DrawString(string text, Vector3 position) {
#if UNITY_EDITOR
        if (TextStyle == null) {
            TextStyle = new GUIStyle(GUI.skin.label) {
                alignment = TextAnchor.MiddleCenter,
                normal = {textColor = Color.white}
            };
        }
        UnityEditor.Handles.Label( position, text, TextStyle );
#endif
    }
}
