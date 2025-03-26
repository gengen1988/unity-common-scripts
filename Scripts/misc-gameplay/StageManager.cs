using System.Collections.Generic;
using Lean.Pool;
using Weaver;

public class StageManager : WeaverSingletonBehaviour<StageManager>, IEntityFrame
{
    [AssetReference] private static readonly StageManager SingletonPrefab;

    private readonly List<GameEntity> _entityInStage = new();

    private Stage _currentStage;
    public Stage CurrentStage => _currentStage;

    public void OnEntityFrame(GameEntity entity, float frameTime)
    {
        _entityInStage.RemoveAll(e => !e);
    }

    public Stage LoadStage(Stage prefab)
    {
        _entityInStage.Clear();
        var go = Instantiate(prefab);
        var stage = go.GetComponent<Stage>();
        _currentStage = stage;
        return stage;
    }

    public void UnloadStage()
    {
        foreach (var entity in _entityInStage)
        {
            // TODO dangerous because it ignored state machine
            PoolUtil.Despawn(entity.Proxy.gameObject);
        }

        if (_currentStage != null)
        {
            Destroy(_currentStage.gameObject);
        }
    }

    public void RegisterEntity(GameEntity entity)
    {
        _entityInStage.Add(entity);
    }
}

public class OnStageChanged : IGlobalEvent
{
}