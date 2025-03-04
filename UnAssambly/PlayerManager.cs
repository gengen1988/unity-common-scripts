using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Weaver;

public class PlayerManager : WeaverSingletonBehaviour<PlayerManager>
{
    [AssetReference] private static readonly PlayerManager SingletonPrefab;

    // [SerializeField] private Actor PlayerActorPrefab;

    private PlayerInput _currentInput;
    private Actor _currentActor;
    private GameEntity _currentPlayerEntity;
    private PlayerInputManager _playerInputManager;
    private InputSystemUIInputModule _inputSystemUIInputModule;

    public GameEntity CurrentPlayerEntity => _currentPlayerEntity;
    public Actor CurrentPlayerActor => _currentActor;
    public PlayerInput CurrentInput => _currentInput;

    private void Awake()
    {
        TryGetComponent(out _playerInputManager);
        TryGetComponent(out _inputSystemUIInputModule);
    }

    private void OnEnable()
    {
        _playerInputManager.onPlayerJoined += HandlePlayerJoined;
        _playerInputManager.onPlayerLeft += HandlePlayerLeft;
    }

    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= HandlePlayerJoined;
        _playerInputManager.onPlayerLeft -= HandlePlayerLeft;
    }

    private void HandlePlayerLeft(PlayerInput playerInput)
    {
        if (_currentInput == playerInput)
        {
            _currentInput = null;
        }
    }

    private void HandlePlayerJoined(PlayerInput playerInput)
    {
        if (_currentInput)
        {
            Debug.LogWarning("Player has already joined");
            return;
        }

        // setup input component
        playerInput.camera = Camera.main;
        playerInput.uiInputModule = _inputSystemUIInputModule;

        // register
        _currentInput = playerInput;
    }

    // public void SpawnPlayerActor(Vector2 spawnPoint)
    // {
    //     var actor = PlayerActorPrefab.Spawn(spawnPoint, Quaternion.identity);
    //     var entity = actor.gameObject.GetEntity();
    //     actor.OnKill += Callback;
    //
    //     _currentActor = actor;
    //     _currentPlayerEntity = entity;
    //
    //     Debug.Log("Player spawned");
    //     GlobalEventBus.Emit<OnPlayerSpawned>();
    //     return;
    //
    //     void Callback()
    //     {
    //         Debug.Log("Player died2");
    //         GlobalEventBus.Emit<OnPlayerDied>();
    //         actor.OnKill -= Callback;
    //     }
    // }
}

public class OnPlayerSpawned : GameEvent
{
}

public class OnPlayerDied : GameEvent
{
}