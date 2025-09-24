using ThisNamespace;
using UnityEngine;


[ExecuteAlways]
public class SimpleOrbit : MonoBehaviour {

    [SerializeField] private CameraParams cp;
    [SerializeField] private Camera c;

    private void Update() {
        if (c == null) {
            return;
        }

        c.SetParams(cp);
    }

    private void OnDrawGizmosSelected() {
        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(cp.yaw, Vector3.up), Vector3.one);
        float radius = .5f;

        Vector3 l = m.MultiplyPoint(cp.trackingPoint + Vector3.left * radius);
        Vector3 r = m.MultiplyPoint(cp.trackingPoint + Vector3.right * radius);
        Vector3 u = m.MultiplyPoint(cp.trackingPoint + Vector3.up * radius);
        Vector3 d = m.MultiplyPoint(cp.trackingPoint + Vector3.down * radius);
        Vector3 f = m.MultiplyPoint(cp.trackingPoint + Vector3.forward * radius);
        Vector3 b = m.MultiplyPoint(cp.trackingPoint + Vector3.back * radius);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(l, r);
        Gizmos.DrawLine(u, d);
        Gizmos.DrawLine(f, b);
    }
}
