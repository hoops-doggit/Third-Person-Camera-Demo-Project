using ThisNamespace;
using UnityEngine;

[System.Serializable]
public class AngularSpring
{
[SerializeField] private float m_Frequency = 2.0f;                  // F
    [SerializeField] private float m_DampingRatio = 0.5f;           // Z
    [SerializeField] private float m_Response = 1.0f;               // K3 / R
    [SerializeField] private float m_DecelerationRate = 1.0f;
    [SerializeField] private AnimationCurve m_ResponseCurve = AnimationCurve.Linear(0,1,1,1);

    private float _currentAngle;
    private float _currentVelocity;

    public float CurrentVelocity => _currentVelocity;

    public float Frequency
    {
        get => m_Frequency;
        set => m_Frequency = Mathf.Max(0.001f, value);
    }

    public float DampingRatio
    {
        get => m_DampingRatio;
        set => m_DampingRatio = Mathf.Clamp01(value);
    }

    public float Response
    {
        get => m_Response;
        set => m_Response = Mathf.Max(0f, value);
    }

    public float CurrentAngle {
        get => _currentAngle;
        set => _currentAngle = MathUtils.Wrap(value, 0, 360.0f);
    }

    public AngularSpring(float initialAngle, float frequency, float dampingRatio, float response = 1f)
    {
        _currentAngle = initialAngle;
        m_Frequency = Mathf.Max(0.001f, frequency);
        m_DampingRatio = Mathf.Clamp01(dampingRatio);
        m_Response = Mathf.Max(0f, response);
    }
    
    public float Update(float targetAngle, float deltaTime, bool useTarget)
    {
        float angularFrequency = 2f * Mathf.PI * m_Frequency;
        float K1 = m_DampingRatio / (Mathf.PI * m_Frequency);
        float K2 = 1f / (angularFrequency * angularFrequency);
        float K3 = m_Response;

        float currentRadians = _currentAngle * Mathf.Deg2Rad;

        if (useTarget)
        {
            // Compute shortest angular difference in radians (always continuous)
            float diff = Mathf.DeltaAngle(_currentAngle, targetAngle) * Mathf.Deg2Rad;

            // Limit acceleration so we don't flip direction suddenly
            float acceleration = (diff - K1 * _currentVelocity) / K2;
            float t = Mathf.InverseLerp(0, 180, diff);
            acceleration *= K3 * m_ResponseCurve.Evaluate(t);

            currentRadians += deltaTime * _currentVelocity;
            _currentVelocity += deltaTime * acceleration;
        }
        else
        {
            // Free damping towards 0 velocity
            _currentVelocity = _currentVelocity.ExponentialDecay(0,m_DecelerationRate, Time.deltaTime);
            currentRadians += deltaTime * _currentVelocity;
        }

        _currentAngle = Mathf.Repeat(currentRadians * Mathf.Rad2Deg, 360f);
        return _currentAngle;
    }

    public void Reset(float initialAngle, float freq, float damping, float response) {
        _currentAngle = initialAngle;
        m_Frequency = Mathf.Max(0.001f, freq);
        m_DampingRatio = Mathf.Clamp01(damping);
        m_Response = Mathf.Max(0f, response);
    }
}
