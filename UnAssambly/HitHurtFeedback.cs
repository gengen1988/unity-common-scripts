using UnityEngine;

public class HitHurtFeedback : MonoBehaviour
{
    [SerializeField] private Feedback FeedbackOnHit;

    private void Awake()
    {
        var hitCtrl = this.EnsureComponent<HitController>();
        hitCtrl.OnHit += HandleHit;
    }

    private void HandleHit(HitEventData evt)
    {
        var position = evt.ContactPoint;
        var rotation = MathUtil.QuaternionByVector(evt.HitVelocity);

        // Debug.Log($"[{Time.frameCount}] create hit feedback {FeedbackOnHit} at {position} rotation {rotation}");
        FeedbackOnHit.Play(position, rotation);

        // test early detach
        // Debug.Log($"[{Time.frameCount}] test early detach");
        // var t = PoolUtil.Spawn(FeedbackOnHit, position, rotation);
        // PoolUtil.Despawn(t);
        // Debug.Log("early detach test end");
    }
}