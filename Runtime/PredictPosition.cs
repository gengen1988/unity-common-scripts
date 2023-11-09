using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PredictPosition : MonoBehaviour
{
    private struct Measure
    {
        public float Time;
        public Vector2 Position;
    }

    private readonly Queue<Measure> _measures = new Queue<Measure>();

    public KinematicCtrl Target { get; private set; }

    private void FixedUpdate()
    {
        if (!Target)
        {
            return;
        }

        // take snapshot (should in fixed update ensuring stable framerate)
        var measure = new Measure
        {
            Position = Target.Position,
            Time = Time.time,
        };

        // add to history
        _measures.Enqueue(measure);
        while (_measures.Count > 2)
        {
            _measures.Dequeue();
        }
    }

    public bool IsReady()
    {
        return Target && _measures.Count >= 2;
    }

    public void TrackTarget(KinematicCtrl target)
    {
        Target = target;
        _measures.Clear();
    }

    public Vector2 Predict(float elapsedTime)
    {
        var velocity = GetVelocity(_measures.First(), _measures.Last());
        var displacement = velocity * elapsedTime;
        var position = _measures.First().Position;
        return position + displacement;
    }

    public Vector2 ReadVelocity()
    {
        return GetVelocity(_measures.First(), _measures.Last());
    }

    private Vector2 GetVelocity(Measure m1, Measure m2)
    {
        return (m1.Position - m2.Position) / (m1.Time - m2.Time);
    }

    private Vector3 GetAcceleration(Measure m1, Measure m2, Measure m3)
    {
        var v1 = GetVelocity(m1, m2);
        var v2 = GetVelocity(m2, m3);
        var t1 = (m1.Time + m2.Time) / 2;
        var t2 = (m2.Time + m3.Time) / 2;
        return (v1 - v2) / (t1 - t2);
    }
}