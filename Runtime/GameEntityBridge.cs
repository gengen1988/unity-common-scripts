using System.Linq;
using UnityEngine;

public class GameEntityBridge : MonoBehaviour
{
    [SerializeField] private FrameType frameGroup = FrameType.Normal;

    private bool _isQuitting;
    private bool _isAttached;

    private readonly GameClock _clock = new();
    private readonly GameEventBus _eventBus = new();

    private IEntityAttach[] _attachHandlers;
    private IEntityDetach[] _detachHandlers;
    private IEntityFrame[] _frameHandlers;
    private GameEntity _entity;

    public bool IsAttached => _isAttached;
    public GameClock Clock => _clock;
    public GameEventBus EventBus => _eventBus;

    private void Awake()
    {
        _attachHandlers = GetComponents<IEntityAttach>();
        _detachHandlers = GetComponents<IEntityDetach>();
        _frameHandlers = GetComponents<IEntityFrame>()
            .OrderBy(handler =>
            {
                var type = handler.GetType();
                var attr = type.GetCustomAttributes(typeof(GameFrameOrderAttribute), false)
                    .FirstOrDefault() as GameFrameOrderAttribute;
                return attr?.Order ?? 0;
            })
            .ToArray();
    }

    private void OnEnable()
    {
        GameWorld.Instance.RegisterEntity(gameObject, frameGroup);
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

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    /**
     * this method is called in frame tick, not related awake or enable
     * <param name="entity"></param>
     */
    public void Attach(GameEntity entity)
    {
        // _eventBus.Reset();
        _clock.Reset();
        foreach (var handler in _attachHandlers)
        {
            handler.OnEntityAttach(entity);
        }

        _isAttached = true;
    }

    public void Detach(GameEntity entity)
    {
        foreach (var handler in _detachHandlers)
        {
            handler.OnEntityDetach(entity);
        }

        _isAttached = false;
    }

    public void Tick(GameEntity entity, float frameTime)
    {
        _clock.Tick(frameTime);
        foreach (var handler in _frameHandlers)
        {
            handler.OnEntityFrame(entity);
        }
    }
}

public interface IEntityAttach
{
    void OnEntityAttach(GameEntity entity);
}

public interface IEntityDetach
{
    void OnEntityDetach(GameEntity entity);
}

public interface IEntityFrame
{
    void OnEntityFrame(GameEntity entity);
}