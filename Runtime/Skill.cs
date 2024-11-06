using UnityEngine;

public class Skill : MonoBehaviour
{
    public event ActorEvent OnMount;
    public event ActorEvent OnUnmount;
    public event ActorEvent OnBegin, OnEnd, OnPerform;

    [SerializeField] private float DefaultCooldownTime;

    private bool _triggered;
    private float _cooldownTime;

    private void OnDestroy()
    {
        OnMount = null;
        OnUnmount = null;
        OnBegin = null;
        OnEnd = null;
        OnPerform = null;
    }

    public void Mount(ActorOld skillOwner)
    {
        OnMount?.Invoke(skillOwner);
    }

    public void Unmount(ActorOld skillOwner)
    {
        OnUnmount?.Invoke(skillOwner);
    }

    public void Tick(ActorOld skillOwner, bool inputState)
    {
        float localDeltaTime = skillOwner.Timer.LocalDeltaTime;
        if (_cooldownTime > 0)
        {
            _cooldownTime -= localDeltaTime;
            return;
        }

        if (!_triggered && inputState)
        {
            OnBegin?.Invoke(skillOwner);
            _cooldownTime = DefaultCooldownTime;
            _triggered = true;
        }
        else if (_triggered && !inputState)
        {
            OnEnd?.Invoke(skillOwner);
            _triggered = false;
        }

        if (_triggered)
        {
            OnPerform?.Invoke(skillOwner);
        }
    }
}

/*
public class ShootSingle
{
    private bool _triggered;
    private float _cooldownTime;

    void HandleMove(Actor moveSubject, float deltaTime)
    {
        if (_cooldownTime > 0)
        {
            _cooldownTime -= deltaTime;
        }

        if (moveSubject.Intent.GetBool(IntentKey.Action1))
        {
            if (!_triggered)
            {
                _skillManager.Perform();
                _triggered = true;
            }
        }
        else
        {
            if (_triggered)
            {
                _triggered = false;
            }
        }
    }
}

public class ShootBurst
{
    private bool _triggered;
    private float _cooldownTime;
    private int _burstRemaining;

    void HandleMove(Actor moveSubject, float deltaTime)
    {
        if (_cooldownTime > 0)
        {
            _cooldownTime -= deltaTime;
        }

        if (_burstRemaining > 0)
        {
            Perform();
        }
        else
        {
            if (moveSubject.Intent.GetBool(IntentKey.Action1))
            {
                if (!_triggered)
                {
                    _burstRemaining = BurstCount;
                    Perform();
                    _triggered = true;
                }
            }
            else
            {
                if (_triggered)
                {
                    _triggered = false;
                }
            }
        }
    }

    private void Perform()
    {
        _skillManager.Perform();
        _burstRemaining--;
        _cooldownTime = CooldownTime;
    }
}

public class ShootAutoFire
{
    private bool _triggered;
    private float _cooldownTime;
    private int _burstRemaining;

    void HandleMove(Actor moveSubject, float deltaTime)
    {
        if (_cooldownTime > 0)
        {
            _cooldownTime -= deltaTime;
            return;
        }

        if (_burstRemaining > 0)
        {
            Perform();
            return;
        }

        bool inputState = moveSubject.Intent.GetBool(IntentKey.Action1);
        if (inputState)
        {
            _burstRemaining = BurstCount;
            Perform();
        }
    }

    private void Perform()
    {
        _skillManager.Perform();
        _burstRemaining--;
        _cooldownTime = CooldownTime;
    }
}

public class ShootFlame
{
    private bool _triggered;
    private float _cooldownTime;
    private int _burstRemaining;

    void HandleMove(Actor moveSubject, float deltaTime)
    {
        if (_cooldownTime > 0)
        {
            _cooldownTime -= deltaTime;
            return;
        }

        if (_burstRemaining > 0)
        {
            Perform();
            return;
        }

        bool inputState = moveSubject.Intent.GetBool(IntentKey.Action1);
        if (_triggered && !inputState)
        {
            Stop();
        }
        else if (!_triggered && inputState)
        {
            Start();
        }

        if (_triggered)
        {
            Perform();
        }
    }

    private void Perform()
    {
        _skill.Perform();
        _burstRemaining--;
        _cooldownTime = CooldownTime;
    }

    private void Start()
    {
        _skill.Start();
        _triggered = true;
    }

    private void Stop()
    {
        _skill.Stop();
        _triggered = false;
    }
}
*/