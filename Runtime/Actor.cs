using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] private float SpawnTime;
    [SerializeField] private float DespawnTime;

    [Header("Feedbacks")] [SerializeField] private Feedback FeedbackOnSpawn;

    private float _elapsedTime;

    private void Awake()
    {
        TryGetComponent(out GameEntity entity);
        entity.OnTick += HandleTick;
        entity.OnSpawn += HandleSpawn;
        entity.OnReady += HandleReady;
        entity.OnKill += HandleKill;
    }

    private void HandleSpawn(GameEntity entity)
    {
        _elapsedTime = 0;
        gameObject.SetActive(false);
        Feedback.Spawn(FeedbackOnSpawn);
    }

    private void HandleReady(GameEntity entity)
    {
        gameObject.SetActive(true);
    }

    private void HandleKill(GameEntity entity)
    {
        _elapsedTime = 0;
    }

    private void HandleTick(GameEntity entity, float deltaTime)
    {
        switch (entity.CurrentState)
        {
            case EntityState.Spawning:
                TickSpawning(entity, deltaTime);
                break;
            case EntityState.Despawning:
                TickDespawning(entity, deltaTime);
                break;
        }
    }

    private void TickSpawning(GameEntity entity, float deltaTime)
    {
        if (_elapsedTime >= SpawnTime)
        {
            entity.SendReady();
            return;
        }

        _elapsedTime += deltaTime;
    }

    private void TickDespawning(GameEntity entity, float deltaTime)
    {
        if (_elapsedTime >= DespawnTime)
        {
            entity.SendFinish();
            return;
        }

        _elapsedTime += deltaTime;
    }

    // static shorthands
    public static Actor Spawn(Actor prefab, Vector3 position, Quaternion rotation)
    {
        if (!prefab)
        {
            return null;
        }

        var go = EntityManager.Instance.Spawn(prefab.gameObject, position, rotation);
        go.TryGetComponent(out Actor actor);
        return actor;
    }

    public static void Kill(Actor actor)
    {
        if (!actor)
        {
            return;
        }

        EntityManager.Instance.Kill(actor.gameObject);
    }

    public static bool IsAlive(Actor actor)
    {
        throw new System.NotImplementedException();
    }
}