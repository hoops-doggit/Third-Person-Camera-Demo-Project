using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCameraDebug : MonoBehaviour {
    [SerializeField] private PlayerCameraV1 pc;
    [SerializeField] private ThirdPersonSettingConfig settings;
    [SerializeField] private ThirdPersonGeneralConfig generalConfig;
    [SerializeField] private Transform t;
    [SerializeField] private int distanceArcDivisions = 20;
    [SerializeField] private Material material;

    private Vector3[] Distance0 = Array.Empty<Vector3>();
    private Vector3[] Distance1 = Array.Empty<Vector3>();
    private Vector3[] Distance2 = Array.Empty<Vector3>();
    private Vector3 _trackingPosition;
    private void OnDrawGizmos() {
        if (t == null) {
            return;
        }

        if (pc == null) {
            return;
        }

        if (settings == null || generalConfig == null) return;

        Gizmos.color = Color.cyan;
        Vector3 playerPosition = t.position;
        _trackingPosition = playerPosition + Vector3.up * settings.TrackingHeight;
        Gizmos.DrawLine(playerPosition, _trackingPosition);
        DrawString("Tracking Position", _trackingPosition);

        Vector3 maxPitchPosition = Quaternion.AngleAxis(generalConfig.MaxPitch, Vector3.right) * (Vector3.back * 3);
        maxPitchPosition += _trackingPosition;
        Gizmos.DrawLine(_trackingPosition, maxPitchPosition);
        DrawString("maxPitch", maxPitchPosition);

        Vector3 minPitchPosition = Quaternion.AngleAxis(generalConfig.MinPitch, Vector3.right) * (Vector3.back * 3);
        minPitchPosition += _trackingPosition;
        Gizmos.DrawLine(_trackingPosition, minPitchPosition);
        DrawString("minPitch", minPitchPosition);
        
        float distance0 = DesiredDistance(0);
        float distance1 = DesiredDistance(1);
        float distance2 = DesiredDistance(2);
        
        if (Distance0.Length != distanceArcDivisions) {
            Distance0 = new Vector3[distanceArcDivisions];
            Distance1 = new Vector3[distanceArcDivisions];
            Distance2 = new Vector3[distanceArcDivisions];
            
            for (int i = 0; i < distanceArcDivisions; i++) {
                float pitch = Mathf.Lerp(generalConfig.MinPitch, generalConfig.MaxPitch, (float)i / distanceArcDivisions);
                float mult = settings.DistanceXPitchCurve.Evaluate(pitch);
                Distance0[i] = Quaternion.AngleAxis(pitch, Vector3.right) * (Vector3.back * (distance0 * mult));
                Distance1[i] = Quaternion.AngleAxis(pitch, Vector3.right) * (Vector3.back * (distance1 * mult));
                Distance2[i] = Quaternion.AngleAxis(pitch, Vector3.right) * (Vector3.back * (distance2 * mult));
            }
        }

        Vector3 a, b;
        for (int i = 1; i < distanceArcDivisions; i++) {
            float pitcha = Mathf.Lerp(generalConfig.MinPitch, generalConfig.MaxPitch, (float)(i-1) / distanceArcDivisions);
            float pitchb = Mathf.Lerp(generalConfig.MinPitch, generalConfig.MaxPitch, (float)i / distanceArcDivisions);
            float aMult = settings.DistanceXPitchCurve.Evaluate(pitcha);
            float bMult = settings.DistanceXPitchCurve.Evaluate(pitchb);
            a = Quaternion.AngleAxis(pitcha, Vector3.right) * (Vector3.back * (distance0 * aMult));
            a += _trackingPosition;
            b = Quaternion.AngleAxis(pitchb, Vector3.right) * (Vector3.back * (distance0 * bMult));
            b += _trackingPosition;
            Gizmos.DrawLine(a, b);
            
            a = Quaternion.AngleAxis(pitcha, Vector3.right) * (Vector3.back * (distance1 * aMult));
            a += _trackingPosition;
            b = Quaternion.AngleAxis(pitchb, Vector3.right) * (Vector3.back * (distance1 * bMult));
            b += _trackingPosition;
            Gizmos.DrawLine(a, b);
            
            a = Quaternion.AngleAxis(pitcha, Vector3.right) * (Vector3.back * (distance2 * aMult));
            a += _trackingPosition;
            b = Quaternion.AngleAxis(pitchb, Vector3.right) * (Vector3.back * (distance2 * bMult));
            b += _trackingPosition;
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

    private void DrawString(string text, Vector3 position, Color color) {
#if UNITY_EDITOR
        Color c = UnityEditor.Handles.color;
        UnityEditor.Handles.color = color;
        UnityEditor.Handles.Label( position, text);
        UnityEditor.Handles.color = c;
#endif
    }

    private GUIStyle TextStyle;
    
    private void DrawString(string text, Vector3 position) {
#if UNITY_EDITOR
        if (TextStyle == null) {
            TextStyle = new GUIStyle(GUI.skin.label) {
                alignment = TextAnchor.MiddleCenter
            };
        }
        UnityEditor.Handles.Label( position, text, TextStyle );
#endif
    }
}
