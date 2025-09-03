#region Information
/*****
* 
* 26.09.2022
* 
* SecondOrderDynamics.cs implementation based on the video source provided by the author - t3ssel8r
* Link to the original YouTube video: https://www.youtube.com/watch?v=KPoeNZZ6H4s&t=554s&ab_channel=t3ssel8r
* 
*****/
#endregion Information

using UnityEngine;
using Unity.Mathematics;

namespace SODynamics
{
    public class SecondOrderDynamicsFloat
    {
        private float? xp;
        private float? y, yd;
        private float _w, _damping, _d, k1, k2, k3;

        public SecondOrderDynamicsFloat(float frequency, float damping, float response, float initialValue)
        {
            _w = 2 * math.PI * frequency;
            _damping = damping;
            _d = _w * math.sqrt(math.abs(damping * damping - 1));
            k1 = damping / (math.PI * frequency);
            k2 = 1 / (_w * _w);
            k3 = response * damping / _w;

            xp = initialValue;
            y = initialValue;
            yd = 0;
        }

        public float? Update(float T, float x, float? xd = null)
        {
            if (xd == null)
            {
                xd = (x - xp) / T;
                xp = x;
            }

            float k1Stable, k2Stable;
            if (_w * T < _damping)
            {
                k1Stable = k1;
                k2Stable = Mathf.Max(k2, T * T / 2 + T * k1 / 2, T * k1);
            }
            else
            {
                float t1 = math.exp(-_damping * _w * T);
                float alpha = 2 * t1 * (_damping <= 1 ? math.cos(T * _d) : math.cosh(T * _d));
                float beta = t1 * t1;
                float t2 = T / (1 + beta - alpha);
                k1Stable = (1 - beta) * t2;
                k2Stable = T * t2;
            }

            y = y + T * yd;
            yd = yd + T * (x + k3 * xd - y - k1Stable * yd) / k2Stable;
            return y;
        }
    }
}