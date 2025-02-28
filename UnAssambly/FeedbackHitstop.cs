using UnityEngine;

[GameFrameOrder(-10)] // before feedback tick
public class FeedbackHitstop : MonoBehaviour, IEntityFrame
{
    [SerializeField] private TimeDomain HitstopDomain;
    [SerializeField] private float HitstopTime = 0.1f;

    private float _elapsedTime;
    private Feedback _feedback;
    private HitEventData _hitEvent;
    private HitController _hitCtrl, _hurtCtrl;
    private GameClock _hitClock, _hurtClock;
    private GameEntityBridge _bridge;

    private void Awake()
    {
        _bridge = this.EnsureComponent<GameEntityBridge>();
        _feedback = this.EnsureComponent<Feedback>();
        _feedback.OnPlay += HandlePlay;
        _feedback.OnStop += HandleStop;
    }

    public void OnEntityFrame(GameEntity entity)
    {
        if (_elapsedTime >= HitstopTime)
        {
            _feedback.ReleaseBlock(this);
            return;
        }

        _elapsedTime += _bridge.Clock.LocalDeltaTime;
    }

    public void Setup(HitEventData eventData)
    {
        _hitEvent = eventData;
        var hitBridge = eventData.HitEntity.GetBridge();
        var hurtBridge = eventData.HurtEntity.GetBridge();
        _hitCtrl = hitBridge.EnsureComponent<HitController>();
        _hurtCtrl = hurtBridge.EnsureComponent<HitController>();
        _hitClock = hitBridge.Clock;
        _hurtClock = hurtBridge.Clock;
    }

    private void HandlePlay()
    {
        _elapsedTime = 0;
        _feedback.AcquireBlock(this);

        // trigger hit event
        if (_hitEvent.HitEntity.IsAlive())
        {
            // create feedbacks (sounds, particles, camera shake, etc ...) in this event
            // Debug.Log($"[{Time.frameCount}] on hit callback invoked: {_hitCtrl}");
            _hitCtrl.NotifyHit(_hitEvent);
            _hitClock.EnterDomain(HitstopDomain, 0);
        }

        // hit stop
        // Debug.Log("hitstop begin");
        if (_hitEvent.HurtEntity.IsAlive())
        {
            // white flash
            var hurtFlash = _hurtCtrl.EnsureComponent<HurtFlash>();
            hurtFlash.Execute();

            // time stop
            _hurtClock.EnterDomain(HitstopDomain, 0);
        }
    }

    private void HandleStop()
    {
        if (_hitEvent.HitEntity.IsAlive())
        {
            _hitClock.LeaveDomain(HitstopDomain);
        }

        // trigger hurt event
        if (_hitEvent.HurtEntity.IsAlive())
        {
            _hurtClock.LeaveDomain(HitstopDomain);
            _hurtCtrl.NotifyHurt(_hitEvent); // damage number in this event
            // Debug.Log($"on hurt callback invoked: {hurtManager}");
        }
    }
}