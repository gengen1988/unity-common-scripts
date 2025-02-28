using UnityEngine;

public class ActorKnockback : MonoBehaviour
{
    private bool _isUncontrol;
    private ArcadeMovement _movement;

    // private float _elapsedTime;
    //
    // public void OnEntityFrame(float frameTime)
    // {
    //     _elapsedTime += frameTime;
    // }

    // private Vector2 _currentKnockBackForce;

    private void Awake()
    {
        _movement = this.EnsureComponent<ArcadeMovement>();
    }

    public void Knockback(Vector2 force)
    {
        _movement.SetVelocity(force);
    }

    public void Recover()
    {
        _isUncontrol = false;
    }
}