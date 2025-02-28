using System;
using UnityEngine;

public class Actor : StatefulEntityBehaviour<Actor>
{
    public const string MSG_KILL = "kill";
    public const string MSG_DOWN = "down";
    public const string MSG_FORCE_RECOVER = "recover";

    // lifespan
    public event Action OnBorn;
    public event Action OnReady;
    public event Action OnKill;
    public event Action OnVanish;

    // tick order
    public event Action OnPerceive;
    public event Action OnControl;
    public event Action OnMove;

    [SerializeField] private float SpawnTime;
    [SerializeField] private float DespawnTime;

    private float _elapsedTime;
    // private bool _isDown;

    // optional components
    private Pawn _pawn;
    private ArcadeMovement _movement;
    private Health _health;

    protected override IState<Actor> InitialState => Spawning.Instance;

    private void Awake()
    {
        TryGetComponent(out _pawn);
        TryGetComponent(out _movement);
        TryGetComponent(out _health);
        if (_health)
        {
            _health.OnTakeDamage += HandleTakeDamage;
        }
    }

    private void OnDestroy()
    {
        OnBorn = null;
        OnReady = null;
        OnKill = null;
        OnVanish = null;
        OnPerceive = null;
        OnControl = null;
        OnMove = null;
    }

    private void HandleTakeDamage(int damage)
    {
        // event notify
        GlobalEventBus.Emit<OnActorDamage>(evt =>
        {
            evt.Actor = this;
            evt.Damage = damage;
        });

        // die
        if (_health.CurrentHP <= 0)
        {
            this.Kill();
        }
    }

    private class Spawning : State<Actor, Spawning>
    {
        public override void OnEnter(Actor ctx)
        {
            ctx._elapsedTime = 0;
            ctx.OnBorn?.Invoke();
        }

        public override void OnRefresh(Actor ctx)
        {
            if (ctx._elapsedTime >= ctx.SpawnTime)
            {
                ctx.TransitionTo(Normal.Instance);

                // event notify
                // ready at least delay one frame allowing others to set up
                ctx.OnReady?.Invoke();
                GlobalEventBus.Emit<OnActorReady>(evt => evt.Actor = ctx);
                return;
            }

            ctx._elapsedTime += ctx.LocalDeltaTime;
        }
    }

    public class Normal : State<Actor, Normal>
    {
        public override void OnEnter(Actor ctx)
        {
            ctx.RegisterTransition(MSG_KILL, Despawning.Instance);
            // ctx.RegisterTransition(MSG_STUN, Stunning.Instance);
        }

        public override void OnRefresh(Actor ctx)
        {
            var deltaTime = ctx.LocalDeltaTime;
            if (deltaTime <= 0)
            {
                return;
            }

            ctx._pawn?.Query();
            ctx.OnPerceive?.Invoke();
            ctx.OnControl?.Invoke();
            ctx.OnMove?.Invoke();
            ctx._movement?.Tick(deltaTime);
            ctx._pawn?.Clear();
        }
    }

    /**
     * hitstop 是时间特效，攻击方与受击方均停止等同的时间
     * stun 是玩法，攻击方停止时间更长
     */
    // public class Stunning : State<Actor, Stunning>
    // {
    //     public override void OnEnter(Actor ctx)
    //     {
    //         ctx.RegisterTransition(MSG_KILL, Despawning.Instance);
    //         ctx._elapsedTime = 0;
    //     }
    //
    //     public override void OnExit(Actor ctx)
    //     {
    //         ctx._stunTime = 0;
    //     }
    //
    //     public override void Tick(Actor ctx, float deltaTime)
    //     {
    //         if (ctx._elapsedTime >= ctx._stunTime)
    //         {
    //             if (ctx._isDown)
    //             {
    //                 ctx.TransitionTo(Down.Instance);
    //             }
    //             else
    //             {
    //                 ctx.TransitionTo(Normal.Instance);
    //             }
    //
    //             return;
    //         }
    //
    //         ctx._pawn.Query();
    //         ctx.OnPerceive?.Invoke(deltaTime); // allow input to cancel stunning
    //         ctx._pawn.Clear();
    //
    //         ctx._elapsedTime += deltaTime;
    //     }
    // }
    public class Down : State<Actor, Down>
    {
        // public override void OnEnter(Actor ctx)
        // {
        //     ctx.RegisterTransition(MSG_KILL, Despawning.Instance);
        //     ctx.RegisterTransition(MSG_FORCE_RECOVER, Normal.Instance);
        //     ctx._elapsedTime = 0;
        // }
        //
        // public override void Tick(Actor ctx, float deltaTime)
        // {
        //     if (ctx._elapsedTime >= ctx.RecoverTime)
        //     {
        //         ctx.TransitionTo(Normal.Instance);
        //         return;
        //     }
        //
        //     ctx._pawn.Query();
        //     ctx.OnPerceive?.Invoke(deltaTime); // allow buff passive effects
        //     ctx._movement.Apply(deltaTime); // allow knock back or environment interactive
        //     ctx._elapsedTime += deltaTime;
        // }
    }

    public class Despawning : State<Actor, Despawning>
    {
        public override void OnEnter(Actor ctx)
        {
            ctx._elapsedTime = 0;
            ctx.OnKill?.Invoke();
            GlobalEventBus.Emit<OnActorKill>(evt => evt.Actor = ctx);
        }

        public override void OnRefresh(Actor ctx)
        {
            var deltaTime = ctx.LocalDeltaTime;

            // do not despawn when hitstop 
            if (deltaTime <= 0)
            {
                return;
            }

            // recycle
            if (ctx._elapsedTime >= ctx.DespawnTime)
            {
                ctx.OnVanish?.Invoke();
                PoolUtil.Despawn(ctx); // auto transition to null when OnDisabled invoked
                return;
            }

            // store time
            ctx._elapsedTime += deltaTime; // despawn time do not scaled by domains?
        }
    }
}

public class OnActorReady : GameEvent
{
    public Actor Actor;
}

public class OnActorKill : GameEvent
{
    public Actor Actor;
}

public class OnActorDamage : GameEvent
{
    public Actor Actor;
    public int Damage;
}