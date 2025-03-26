using System.Linq;
using UnityEngine;

/// <summary>
/// event order:
/// - (spawn)
/// - on enable
/// - (other logic that after spawn object)
/// - (until fixed update end in same frame)
/// - on attach
/// - (regular game logic)
/// - (despawn)
/// - on disable
/// - on detach (immediately)
/// </summary>
public class EntityProxy : MonoBehaviour
{
    private bool _isQuitting;

    private IEntityAttach[] _attachHandlers;
    private IEntityDetach[] _detachHandlers;
    private IEntityFrame[] _frameHandlers;

    private readonly GameClock _clock = new();

    public GameClock Clock => _clock;
    public GameEntity Entity => GameWorld.Instance.GetEntity(gameObject);

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    private void Awake()
    {
        _attachHandlers = GetComponents<IEntityAttach>();
        _detachHandlers = GetComponents<IEntityDetach>();
        _frameHandlers = GetComponents<IEntityFrame>()
            .OrderBy(handler =>
            {
                var type = handler.GetType();
                var attr = type.GetCustomAttributes(typeof(EntityFrameOrderAttribute), false)
                    .FirstOrDefault() as EntityFrameOrderAttribute;
                return attr?.Order ?? FrameOrder.Default;
            })
            .ToArray();
    }

    private void OnEnable()
    {
        GameWorld.Instance.RegisterEntity(gameObject);
    }

    private void OnDisable()
    {
        // prevent spawn GameWorld when quitting
        if (_isQuitting)
        {
            return;
        }

        GameWorld.Instance.DeregisterEntity(gameObject);
    }

    public void Attach(GameEntity entity)
    {
        _clock.Reset();
        foreach (var handler in _attachHandlers)
        {
            handler.OnEntityAttach(entity);
        }
    }

    public void Detach(GameEntity entity)
    {
        foreach (var handler in _detachHandlers)
        {
            handler.OnEntityDetach(entity);
        }
    }

    public void Tick(GameEntity entity, float frameTime)
    {
        _clock.Tick(frameTime);
        var localDeltaTime = _clock.LocalDeltaTime;

        // do not tick pause object
        if (localDeltaTime <= 0)
        {
            return;
        }

        foreach (var handler in _frameHandlers)
        {
            handler.OnEntityFrame(entity, localDeltaTime);
        }
    }
}

public interface IEntityAttach
{
    public void OnEntityAttach(GameEntity entity);
}

public interface IEntityDetach
{
    public void OnEntityDetach(GameEntity entity);
}

public interface IEntityFrame
{
    public void OnEntityFrame(GameEntity entity, float deltaTime);
}