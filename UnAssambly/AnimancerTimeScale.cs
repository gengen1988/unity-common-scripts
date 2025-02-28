using Animancer;
using UnityEngine;

[GameFrameOrder(-90)] // after time controller update
public class AnimancerTimeScale : MonoBehaviour, IEntityFrame
{
    private Animator _animator;
    private AnimancerComponent _animancer;

    private void Awake()
    {
        _animancer = this.EnsureComponent<AnimancerComponent>();
        _animator = this.EnsureComponent<Animator>();
        _animator.writeDefaultValuesOnDisable = true;
    }

    public void OnEntityFrame(GameEntity entity)
    {
        var bridge = entity.GetBridge();
        _animancer.Graph.Speed = bridge.Clock.LocalTimeScale;
    }
}