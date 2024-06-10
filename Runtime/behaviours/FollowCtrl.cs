using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowCtrl : MonoBehaviour
{
    private struct TransformHistory
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float DeltaTime;
    }

    public Transform[] Segments;
    public float DelayTime;

    private Queue<TransformHistory>[] _history;

    private void Awake()
    {
        _history = new Queue<TransformHistory>[Segments.Length];
        for (int i = 0; i < Segments.Length; ++i)
        {
            _history[i] = new Queue<TransformHistory>();
        }
    }

    private void FixedUpdate()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(float deltaTime)
    {
        // record history
        for (int i = 0; i < Segments.Length; ++i)
        {
            Queue<TransformHistory> history = _history[i];
            Transform segment = Segments[i];
            TransformHistory th = new()
            {
                Position = segment.position,
                Rotation = segment.rotation,
                DeltaTime = deltaTime,
            };
            history.Enqueue(th);

            // remove unused
            while (history.Sum(el => el.DeltaTime) > DelayTime)
            {
                history.Dequeue();
            }
        }

        // apply new position
        for (int i = 0; i < Segments.Length; ++i)
        {
            Vector3 targetPosition = GetTargetPosition(i);
            Quaternion targetRotation = GetTargetRotation(i);
            if (Segments[i].TryGetComponent(out Rigidbody2D rb))
            {
                rb.MovePosition(targetPosition);
                rb.MoveRotation(targetRotation);
            }
            else
            {
                Segments[i].position = targetPosition;
                Segments[i].rotation = targetRotation;
            }
        }
    }

    private Vector3 GetTargetPosition(int i)
    {
        return i == 0 ? transform.position : _history[i - 1].Peek().Position;
    }

    private Quaternion GetTargetRotation(int i)
    {
        return i == 0 ? transform.rotation : _history[i - 1].Peek().Rotation;
    }
}