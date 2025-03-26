using UnityEngine;
using Weaver;

public class BattleManager : WeaverSingletonBehaviour<BattleManager>
{
    [AssetReference] private static readonly BattleManager SingletonPrefab;

    private GameEntity _currentPlayerGameObject;

    public void SetPlayerGameObject(GameObject go)
    {
        _currentPlayerGameObject = go.GetEntity();
    }

    public GameObject GetPlayerGameObject()
    {
        return _currentPlayerGameObject ? _currentPlayerGameObject.Proxy.gameObject : null;
    }
}