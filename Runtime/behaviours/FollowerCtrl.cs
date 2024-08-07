﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * fixed update order
 * -300: perception
 * -200: ctrl
 * -100: fixed logic calculation (constraint)
 *    0: rigidbody move position or transform change
 *  100: after movement
 */
[DefaultExecutionOrder(100)]
public class FollowerCtrl : MonoBehaviour
{
    public float DelayTime = 0.2f;
    public float StoreOctave = 1f;
    public GameObject[] Segments;

    private readonly LinkedList<TransformSnapshot> _history = new();

    private void FixedUpdate()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(float deltaTime)
    {
        // record history
        Transform head = transform;
        _history.AddFirst(new TransformSnapshot
        {
            Position = head.position,
            Rotation = head.rotation,
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

                MoveSegment(index, snapshot);
                index++;

                lastSnapshot = snapshot;
                cooldownTime = DelayTime; // might not precise, but looks more even (use += for a precise result)
            }

            cooldownTime -= snapshot.DeltaTime;
        }

        // setup remaining segments
        while (index < Segments.Length)
        {
            MoveSegment(index, lastSnapshot);
            index++;
        }
    }

    private void MoveSegment(int index, TransformSnapshot snapshot)
    {
        GameObject go = Segments[index];
        if (!go)
        {
            return;
        }

        // move actor
        if (go.TryGetComponent(out Rigidbody2D rb))
        {
            rb.MovePosition(snapshot.Position);
            rb.MoveRotation(snapshot.Rotation);
        }
        // move slot
        else
        {
            Transform slot = go.transform;
            slot.position = snapshot.Position;
            slot.rotation = snapshot.Rotation;
        }
    }
}