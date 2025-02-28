using UnityEngine;

/**
 * source: https://www.patreon.com/posts/procedural-video-82487305
 */
public class SecondOrderDynamics
{
    private readonly float _k1, _k2, _k3;
    private readonly float _d, _w, _z;

    private Vector3 _x;
    private Vector3 _y, _yd;

    public SecondOrderDynamics(float f, float z, float r, Vector3 x0)
    {
        _w = 2 * Mathf.PI * f;
        _z = z;
        _d = _w * Mathf.Sqrt(Mathf.Abs(z * z - 1));

        _k1 = 0.5f * z / _w;
        _k2 = 1 / (_w * _w);
        _k3 = r * z / _w;

        // initial states
        _x = x0;
        _y = x0;
        _yd = Vector3.zero;
    }

    public Vector3 Tick(Vector3 xNext, float deltaTime)
    {
        // x velocity
        var xdNext = (xNext - _x) / deltaTime;

        // float k1Stable = _k1, k2Stable = _k2;
        // if (_w * deltaTime < _z)
        // {
        //     // clamp k2 to guarantee stability without jitter
        //     k2Stable = Mathf.Max(
        //         _k2,
        //         deltaTime * deltaTime / 2 + deltaTime * _k1 / 2,
        //         deltaTime * _k1
        //     );
        // }

        // float k1Stable, k2Stable;
        // if (_w * deltaTime < _z)
        // {
        //     // clamp k2 to guarantee stability without jitter
        //     k1Stable = _k1;
        //     k2Stable = Mathf.Max(
        //         _k2,
        //         deltaTime * deltaTime / 2 + deltaTime * _k1 / 2,
        //         deltaTime * _k1
        //     );
        // }
        // else
        // {
        //     // use pole matching when the system is very fast
        //     float n = _z <= 1
        //         ? MathF.Cos(deltaTime * _d)
        //         : MathF.Cosh(deltaTime * _d);
        //     float t1 = Mathf.Exp(-_z * _w * deltaTime);
        //     float alpha = 2 * t1 * n;
        //     float beta = t1 * t1;
        //     float t2 = deltaTime / (1 + beta - alpha);
        //     k1Stable = (1 - beta) * t2;
        //     k2Stable = deltaTime * t2;
        // }

        // semi-implicit euler
        // ẏ[n+1] = ẏ[n] + T(x[n+1] + k₃ẋ[n+1] - y[n+1] - k₁ẏ[n]) / k₂
        // Vector3 yNext = _y + deltaTime * _yd;
        // Vector3 ydNext = _yd + deltaTime * (xNext + _k3 * xdNext - yNext - k1Stable * _yd) / k2Stable;

        // backward euler
        // ẏ[n+1] = ẏ[n] + T(x[n+1] + k₃ẋ[n+1] - y[n+1] - k₁ẏ[n+1]) / k₂
        // ẏ[n+1] = (k₂ẏ[n] + T(x[n+1] + k₃ẋ[n+1] - y[n+1])) / (k₂ + Tk₁)
        var yNext = _y + deltaTime * _yd;
        var ydNext = (_k2 * _yd + deltaTime * (xNext + _k3 * xdNext - yNext)) / (_k2 + deltaTime * _k1);

        // update states
        _x = xNext;
        _y = yNext;
        _yd = ydNext;

        return yNext;
    }
}