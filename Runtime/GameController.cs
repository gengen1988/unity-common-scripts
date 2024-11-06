using UnityEngine;

public class GameController : SingletonBehaviour<GameController>
{
    public ActorOld PlayerPrefab;

    private ActorOld _player;

    public ActorOld CurrentPlayer => _player;

    private void Update()
    {
        if (!_player)
        {
            _player = ActorOld.Spawn(PlayerPrefab, Vector2.zero, Quaternion.identity);
        }
    }

    public int GetEnemyCount()
    {
        throw new System.NotImplementedException();
    }

    public void GetCurrentStage()
    {
        throw new System.NotImplementedException();
    }
}