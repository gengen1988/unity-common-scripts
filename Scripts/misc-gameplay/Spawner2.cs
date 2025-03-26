using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

// - pick what to spawn
// - place to position
// - manage amount
public class Spawner2 : MonoBehaviour, IEntityFrame
{
    [Header("Time")]
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private float anticipateTime;
    [SerializeField] private float cooldownTime = 1f;

    [Header("Amount")]
    [SerializeField] private int burstAmount = 1;
    [SerializeField] private float maxAmount = 3f;

    [Header("Strategy")]
    [SerializeReference] private IPickingSpawnStrategy pickingStrategy;
    [SerializeReference] private IPlacementStrategy placementStrategy;

    [Header("Gameplay Cues")]
    [SerializeField] private CueChannel cueSpawnAnticipate;
    [SerializeField] private CueChannel cueSpawnAppear;

    private float _cooldownTime;
    private bool _isSpawning;
    private int _pendingAmount;
    private CancellationTokenSource _cts;
    private IFFTransponder _iffTransponder;

    private readonly List<GameEntity> _managed = new();

    private void OnDrawGizmosSelected()
    {
        if (placementStrategy != null)
        {
            var center = transform.position;
            placementStrategy.DrawGizmos(center);
        }
    }

    private void Awake()
    {
        TryGetComponent(out _iffTransponder);
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            StartSpawning();
        }
    }

    public void OnEntityFrame(GameEntity entity, float frameTime)
    {
        if (!_isSpawning)
        {
            return;
        }

        if (burstAmount <= 0)
        {
            Debug.LogWarning("BurstAmount is zero, never spawn", this);
        }

        // produce
        if (_cooldownTime <= 0)
        {
            // cleanup
            _managed.RemoveAll(e => !e);

            for (var i = 0; i < burstAmount; i++)
            {
                // stop if spawned too many
                if (_managed.Count + _pendingAmount < maxAmount)
                {
                    SpawnTask(_cts.Token).Forget();
                }
            }

            _cooldownTime = cooldownTime;
        }
        else
        {
            _cooldownTime -= frameTime;
        }
    }

    private async UniTask SpawnTask(CancellationToken ct = default)
    {
        var id = Guid.Empty;
        var position = placementStrategy.SetPosition(transform.position);
        try
        {
            // anticipate
            _pendingAmount++;
            id = cueSpawnAnticipate.PlayIfNotNull(position);

            // OnSpawnBegin?.Invoke(id, position);
            if (anticipateTime > 0)
            {
                await UniTask.WaitForSeconds(anticipateTime, cancellationToken: ct);
            }

            // spawn
            var instance = Spawn(position);

            // add to manage list
            var newEntity = instance.GetEntity();
            _managed.Add(newEntity);
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
        finally
        {
            // notify ready
            cueSpawnAnticipate.StopIfNotNull(id);
            cueSpawnAppear.PlayIfNotNull(position);
            _pendingAmount--;
        }
    }

    private GameObject Spawn(Vector2 location)
    {
        var entry = pickingStrategy.Pick();
        var instance = entry.Spawn();

        // placement
        instance.transform.position = location;

        // copy faction
        if (_iffTransponder)
        {
            var instanceTransponder = instance.EnsureComponent<IFFTransponder>();
            instanceTransponder.Identity = _iffTransponder.Identity;
            // Log faction identity information
            // Debug.Log($"Spawned entity with faction identity: {_iffTransponder.Identity}");
        }

        return instance;
    }

    public void StartSpawning()
    {
        _pendingAmount = 0;
        _cooldownTime = 0;
        _cts = new CancellationTokenSource();
        _isSpawning = true;
    }

    public void StopSpawning()
    {
        _cts.Cancel();
        _cts.Dispose();
        _isSpawning = false;
    }

    public GameObject SpawnUnmanaged()
    {
        var location = placementStrategy.SetPosition(transform.position);
        return Spawn(location);
    }
}

public interface IPickingSpawnStrategy
{
    public ISpawnable Pick();
}

[Serializable]
public class DirectPick : IPickingSpawnStrategy
{
    [SerializeReference] private ISpawnable entry;

    public ISpawnable Pick()
    {
        return entry;
    }
}

[Serializable]
public class SequenceNoStreak : IPickingSpawnStrategy
{
    [Serializable]
    public struct Entry
    {
        [SerializeReference] public ISpawnable toBeSelected;
        public int amount;
    }

    [SerializeField] private Entry[] entries;
    [SerializeField] private int maxSteak = 3;

    private bool _initialized;
    private RandomSequence<ISpawnable> _sequence;
    private RandomMaxStreak<ISpawnable> _streak;

    private void Initialize()
    {
        var allElements = new List<ISpawnable>();
        foreach (var entry in entries)
        {
            var elements = Enumerable.Repeat(entry.toBeSelected, entry.amount);
            allElements.AddRange(elements);
        }

        _sequence = new RandomSequence<ISpawnable>(allElements);
        _streak = new RandomMaxStreak<ISpawnable>(maxSteak);
        _initialized = true;
    }

    public ISpawnable Pick()
    {
        if (!_initialized)
        {
            Initialize();
        }

        var selected = _streak.Next(() => _sequence.CycleDraw());
        return selected;
    }
}

public interface ISpawnable
{
    public GameObject Spawn();
}

[Serializable]
public class UnitSpawnable : ISpawnable
{
    public Unit Prefab;

    public GameObject Spawn()
    {
        var actor = LeanPool.Spawn(Prefab);
        return actor.gameObject;
    }
}

public interface IPlacementStrategy
{
    void DrawGizmos(Vector2 origin);
    Vector2 SetPosition(Vector2 origin);
}

[Serializable]
public class PlaceToOrigin : IPlacementStrategy
{
    public void DrawGizmos(Vector2 origin)
    {
        GizmosUtil.DrawCross2D(origin);
    }

    public Vector2 SetPosition(Vector2 origin)
    {
        return origin;
    }
}

[Serializable]
public class PlacementAroundOrigin : IPlacementStrategy
{
    private const int MAX_REROLL = 20;

    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float overlapSize = 1f;
    [SerializeField] private LayerMask noOverlapMask = 1; // default

    public void DrawGizmos(Vector2 origin)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, minDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(origin, maxDistance);
    }

    public Vector2 SetPosition(Vector2 origin)
    {
        var position = Vector2.zero;
        var positionValid = false;
        for (var i = 0; i < MAX_REROLL && !positionValid; i++)
        {
            var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            var radius = Random.Range(minDistance, maxDistance);
            position = origin + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

            if (!Physics2D.OverlapCircle(position, overlapSize, noOverlapMask))
            {
                positionValid = true;
            }
        }

        if (!positionValid)
        {
            Debug.LogWarning("Failed to find a valid position after " + MAX_REROLL + " attempts.");
        }

        return position;
    }
}

[Serializable]
public class PlacementDirectional : IPlacementStrategy
{
    private const int MAX_REROLL = 20;

    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float overlapSize = 1f;
    [SerializeField] [Range(0, 360)] private float angleDirection = 90;
    [SerializeField] [Range(0, 360)] private float angleRange = 180;
    [SerializeField] private LayerMask noOverlapMask = 1; // default

    public void DrawGizmos(Vector2 origin)
    {
        var direction = MathUtil.VectorByAngle(angleDirection);
        var from = Quaternion.Euler(0, 0, -angleRange / 2) * direction;
        GizmosUtil.DrawWireFan2D(origin, from, angleRange, minDistance, maxDistance);
    }

    public Vector2 SetPosition(Vector2 origin)
    {
        var position = Vector2.zero;
        var positionValid = false;
        for (var i = 0; i < MAX_REROLL && !positionValid; i++)
        {
            var angleFrom = angleDirection - angleRange / 2;
            var angleTo = angleDirection + angleRange / 2;
            var angle = Random.Range(angleFrom, angleTo) * Mathf.Deg2Rad;
            var radius = Random.Range(minDistance, maxDistance);
            position = origin + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

            if (!Physics2D.OverlapCircle(position, overlapSize, noOverlapMask))
            {
                positionValid = true;
            }
        }

        if (!positionValid)
        {
            Debug.LogWarning("Failed to find a valid position after " + MAX_REROLL + " attempts.");
        }

        return position;
    }
}