using UnityEngine;
using UnityEngine.InputSystem;

public class LearningPlayerInputManager : MonoBehaviour
{
    private PlayerInputManager _playerInputManager;

    private void Awake()
    {
        TryGetComponent(out _playerInputManager);
        _playerInputManager.onPlayerJoined += HandlePlayerJoined;
    }

    private void HandlePlayerJoined(PlayerInput obj)
    {
        Debug.Log($"PlayerJoined: {obj}", obj);
    }
}