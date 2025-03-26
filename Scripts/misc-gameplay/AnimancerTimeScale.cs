using Animancer;
using UnityEngine;

[DefaultExecutionOrder(CustomExecutionOrder.TooLate)] // after entity frame
public class AnimancerTimeScale : MonoBehaviour
{
    private AnimancerComponent _animancer;
    private EntityProxy _proxy;

    private void Awake()
    {
        _proxy = GetComponentInParent<EntityProxy>();
        TryGetComponent(out _animancer);
    }

    private void FixedUpdate()
    {
        var targetSpeed = _proxy.Clock.LocalDeltaTime;
        if (Mathf.Approximately(_animancer.Graph.Speed, targetSpeed))
        {
            _animancer.Graph.Speed = targetSpeed;
        }
    }
}