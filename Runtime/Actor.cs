using UnityEngine;

public delegate void ActorEvent(Actor actor);

public class Actor : StatefulBehaviour<Actor>
{
    private const string MESSAGE_KILL = "kill";

    public event ActorEvent OnReady;
    public event ActorEvent OnPerceive;
    public event ActorEvent OnControl;
    public event ActorEvent OnMove;
    public event ActorEvent OnDie;

    [SerializeField] private float DefaultBornTime;
    [SerializeField] private float DefaultDieTime;
    [SerializeField] private Vector2 CenterOffset;

    private float _elapsedTime;

    // components
    private readonly ActorIntent _intent = new();
    private readonly ActorTimer _timer = new();

    public Vector2 CenterPosition => (Vector2)transform.position + CenterOffset;
    public ActorTimer Timer => _timer;
    public ActorIntent Intent => _intent;
    public float BornTime => DefaultBornTime;
    public float DieTime => DefaultDieTime;

    private void Start()
    {
        // in scene actors are normal and skip born
        if (StateMachine.GetCurrentState() == null)
        {
            StateMachine.TransitionTo(Normal.Instance);
        }
    }

    private void FixedUpdate()
    {
        StateMachine.SendMessage();
    }

    private void OnDestroy()
    {
        OnReady = null;
        OnPerceive = null;
        OnControl = null;
        OnMove = null;
        OnDie = null;
    }

    public void Kill()
    {
        StateMachine.SendMessage(MESSAGE_KILL);
    }

    // static interface
    public static Actor Spawn(Actor prefab, Vector2 position, Quaternion rotation)
    {
        if (!prefab)
        {
            return null;
        }

        var go = PoolUtil.Spawn(prefab.gameObject, position, rotation);
        go.TryGetComponent(out Actor actor);
        actor.StateMachine.TransitionTo(Born.Instance);
        return actor;
    }

    public static void Despawn(Actor actor)
    {
        if (!actor)
        {
            return;
        }

        actor.Kill();
    }

    public static bool IsAlive(Actor actor)
    {
        return actor.StateMachine.GetCurrentState() == Normal.Instance;
    }

    private class Born : State<Actor, Born>
    {
        public override void OnEnter(Actor ctx)
        {
            ctx._elapsedTime = 0;
        }

        public override void OnMessage(Actor ctx, string message)
        {
            if (ctx._elapsedTime >= ctx.DefaultBornTime)
            {
                ctx.StateMachine.TransitionTo(Normal.Instance);
                return;
            }

            ctx._elapsedTime += Time.fixedDeltaTime;
        }
    }

    private class Normal : State<Actor, Normal>
    {
        public override void OnEnter(Actor ctx)
        {
            ctx._timer.Cleanup();
            ctx.OnReady?.Invoke(ctx);
        }

        public override void OnMessage(Actor ctx, string message)
        {
            if (message == MESSAGE_KILL)
            {
                ctx.StateMachine.TransitionTo(Died.Instance);
                return;
            }

            var deltaTime = Time.fixedDeltaTime;
            ctx._intent.Cleanup();
            ctx._timer.Perceive(deltaTime);

            // game logic
            ctx.OnPerceive?.Invoke(ctx);
            ctx.OnControl?.Invoke(ctx);
            ctx.OnMove?.Invoke(ctx);
        }
    }

    private class Died : State<Actor, Died>
    {
        public override void OnEnter(Actor ctx)
        {
            ctx._elapsedTime = 0;
            ctx.OnDie?.Invoke(ctx);
        }

        public override void OnMessage(Actor ctx, string message)
        {
            if (ctx._elapsedTime >= ctx.DefaultDieTime)
            {
                PoolUtil.Despawn(ctx.gameObject);
                return;
            }

            ctx._elapsedTime += Time.fixedDeltaTime;
        }
    }
}

// /**
//  * actor components shortcut
//  */
// public partial class Actor
// {
//     public ActorBuffManager BuffManager => TryGetComponent(out ActorBuffManager buffManager) ? buffManager : null;
//     public ActorHealth Health => TryGetComponent(out ActorHealth health) ? health : null;
//     public AttributeManager Attribute => TryGetComponent(out AttributeManager attribute) ? attribute : null;
//     public ActorNonPenetration NonPenetration => TryGetComponent(out ActorNonPenetration movement) ? movement : null;
//     public ActorHitManager HitManager => TryGetComponent(out ActorHitManager hit) ? hit : null;
// }