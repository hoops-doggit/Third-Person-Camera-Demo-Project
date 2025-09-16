using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class SecondOrderDynamicsAngle
{
    [SerializeField] private float m_Frequency;
    [SerializeField] private float m_DampingRatio;
    [SerializeField] private float m_Response;

    private float xp;   // previous target angle
    private float y;    // current angle
    private float yd;   // current angular velocity

    private float _w, _d, k1, k2, k3;

    public float Frequency => m_Frequency;
    public float DampingRatio => m_DampingRatio;
    public float Response => m_Response;
    public float CurrentVelocity => yd;

    private float f, d, r;

    public SecondOrderDynamicsAngle(float frequency, float damping, float response, float initialValue)
    {
        m_Frequency = frequency;
        m_DampingRatio = damping;
        m_Response = response;
        f = m_Frequency;
        d = m_DampingRatio;
        r = m_Response;

        _w = 2 * Mathf.PI * m_Frequency;
        _d = _w * math.sqrt(math.abs(m_DampingRatio * m_DampingRatio - 1));
        k1 = m_DampingRatio / (Mathf.PI * m_Frequency);
        k2 = 1 / (_w * _w);
        k3 = m_Response * m_DampingRatio / _w;

        xp = initialValue;
        y = initialValue;
        yd = 0f;
    }

    public float Update(float T, float x, bool followTarget = true, float xd = 0f)
    {
        if (!Mathf.Approximately(f, m_Frequency) || !Mathf.Approximately(d, m_DampingRatio) || !Mathf.Approximately(r, m_Response)) {
            Init(y);
        }
        
        
        if (followTarget)
        {
            float deltaAngle = Mathf.DeltaAngle(xp, x);
            xd = deltaAngle / T;
            xp = x;
        }
        else
        {
            xd = 0f;
        }

        float k1Stable, k2Stable;

        if (_w * T < d)
        {
            k1Stable = k1;
            k2Stable = Mathf.Max(k2, T * T / 2 + T * k1 / 2, T * k1);
        }
        else
        {
            float t1 = math.exp(-d * _w * T);
            float alpha = 2 * t1 * (d <= 1 ? math.cos(T * _d) : math.cosh(T * _d));
            float beta = t1 * t1;
            float t2 = T / (1 + beta - alpha);
            k1Stable = (1 - beta) * t2;
            k2Stable = T * t2;
        }

        y += T * yd;

        if (followTarget)
        {
            float angularError = Mathf.DeltaAngle(y, x);
            yd += T * (angularError + k3 * xd - k1Stable * yd) / k2Stable;
        }
        else
        {
            yd += T * (-k1Stable * yd) / k2Stable;
        }

        y = Mathf.Repeat(y, 360f);

        return y;
    }

    public void Init(float initialValue) {
        f = m_Frequency;
        d = m_DampingRatio;
        r = m_Response;
        
        _w = 2 * Mathf.PI * f;
        _d = _w * math.sqrt(math.abs(d * d - 1));
        k1 = d / (Mathf.PI * f);
        k2 = 1 / (_w * _w);
        k3 = r * d / _w;

        xp = initialValue;
        y = initialValue;
        yd = 0f;
    }
}
