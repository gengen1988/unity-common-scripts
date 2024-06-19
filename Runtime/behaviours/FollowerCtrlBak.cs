using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Obsolete]
[DefaultExecutionOrder(100)]
public class FollowerCtrlBak : MonoBehaviour
{
    public float DelayTime = 0.2f;
    public float StoreOctave = 1f;
    public Transform[] Segments;

    private readonly LinkedList<TransformSnapshot> _history = new();

    private void FixedUpdate()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(float deltaTime)
    {
        // follow target
        Vector3 headPosition = transform.position;

        // record history
        _history.AddFirst(new TransformSnapshot
        {
            Position = headPosition,
            DeltaTime = deltaTime,
        });
        float maxTime = DelayTime * Segments.Length * StoreOctave;
        while (_history.Count > 1 && _history.Sum(s => s.DeltaTime) > maxTime)
        {
            _history.RemoveLast();
        }

        // apply new position
        float cooldownTime = 0;
        int index = 0;
        TransformSnapshot lastSnapshot = _history.First.Value;
        foreach (TransformSnapshot snapshot in _history)
        {
            while (MathUtil.FixedPointRound(cooldownTime) <= 0 && DelayTime > 0)
            {
                if (index >= Segments.Length)
                {
                    break;
                }

                SetPosition(index, snapshot.Position);
                index++;

                lastSnapshot = snapshot;
                cooldownTime = DelayTime; // might not precise, but looks more even (use += for a precise result)
            }

            cooldownTime -= snapshot.DeltaTime;
        }

        // setup remaining segments
        while (index < Segments.Length)
        {
            SetPosition(index, lastSnapshot.Position);
            index++;
        }
    }

    private void SetPosition(int index, Vector3 position)
    {
        Transform segment = Segments[index];
        segment.position = position;
    }
}