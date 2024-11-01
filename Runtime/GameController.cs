using UnityEngine;

public class GameController : SingletonBehaviour<GameController>
{
    public Actor PlayerPrefab;

    private Actor _player;

    public Actor CurrentPlayer => _player;

    private void Update()
    {
        if (!_player)
        {
            _player = Actor.Spawn(PlayerPrefab, Vector2.zero, Quaternion.identity);
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