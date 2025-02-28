using Animancer;
using UnityEngine;

public class HurtFlash : MonoBehaviour
{
    [SerializeField] private AnimationClip HurtFlashAnimation;

    private AnimancerComponent _animancer;

    private void Awake()
    {
        _animancer = this.EnsureComponent<AnimancerComponent>();
    }

    public void Execute()
    {
        // var anime = _animancer.Layers[AnimancerLayers.HIT_EFFECTS].Play(HurtFlashAnimation);
        // anime.Time = 0; // always restart
    }
}