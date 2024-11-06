#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

public class BrainInput : MonoBehaviour
{
    [SerializeField] private string Move = "Player/Move";
    [SerializeField] private string Look = "Player/Look";
    [SerializeField] private string Fire = "Player/Fire";
    [SerializeField] private string Jump = "Player/Jump";
    [SerializeField] private Transform MouseLookCenter;
    [SerializeField] private float MouseLookDeadZone = 0.5f;

    private PlayerInputManager _playerManager;
    private PlayerInput _player;
    private InputAction _moveAction;
    private InputAction _aimAction;
    private InputAction _jumpAction;
    private InputAction _fireAction;

    private bool _jump;
    private bool _fire;

    private void Awake()
    {
        TryGetComponent(out Brain brain);
        // brain.OnMount += HandleMount;
        // brain.OnUnmount += HandleUnmount;
    }

    private void Update()
    {
        if (!_player)
        {
            return;
        }

        // cache until actor tick
        if (_jumpAction.IsPressed())
        {
            _jump = true;
        }

        if (_fireAction.IsPressed())
        {
            _fire = true;
        }
    }

    private void HandleMount(Actor actor)
    {
        // _playerManager = PlayerInputManager.instance;
        // if (_playerManager.playerCount > 0)
        // {
        //     Init(PlayerInput.GetPlayerByIndex(0));
        // }
        //
        // _playerManager.onPlayerJoined += HandlePlayerJoined;
        // _playerManager.onPlayerLeft += HandlerPlayerLeft;
        // actor.OnPerceive += HandlePerceive;
    }

    private void HandleUnmount(Actor actor)
    {
        // actor.OnPerceive -= HandlePerceive;
        // _playerManager.onPlayerJoined -= HandlePlayerJoined;
        // _playerManager.onPlayerLeft -= HandlerPlayerLeft;
    }


    private void HandlePerceive(Actor actor)
    {
        // if (!_player)
        // {
        //     return;
        // }
        //
        // // move
        // actor.Intent.SetVector2(IntentKey.Move, _moveAction.ReadValue<Vector2>());
        //
        // // mouse look 2d
        // var aim = Vector2.zero;
        // if (MouseLookCenter)
        // {
        //     var mousePosition = UnityUtil.GetMousePositionWorld();
        //     var center = (Vector2)MouseLookCenter.position;
        //     var los = mousePosition - center;
        //     if (los.magnitude >= MouseLookDeadZone)
        //     {
        //         aim = los.normalized;
        //     }
        // }
        // // gamepad look
        // else
        // {
        //     aim = _aimAction.ReadValue<Vector2>();
        // }
        //
        // actor.Intent.SetVector2(IntentKey.Look, aim);
        //
        // // fire
        // if (_fire)
        // {
        //     actor.Intent.SetBool(IntentKey.Fire, true);
        //     _fire = false;
        // }
        //
        // // jump
        // if (_jump)
        // {
        //     actor.Intent.SetBool(IntentKey.Jump, true);
        //     _jump = false;
        // }
    }

    private void HandlePlayerJoined(PlayerInput playerInput)
    {
        if (_player)
        {
            Debug.Log("already has player");
            return;
        }

        Init(playerInput);
    }

    private void HandlerPlayerLeft(PlayerInput playerInput)
    {
        if (_player && playerInput == _player)
        {
            _player = null;
        }
    }

    private void Init(PlayerInput playerInput)
    {
        _player = playerInput;
        _moveAction = _player.actions.FindAction(Move, true);
        _aimAction = _player.actions.FindAction(Look, true);
        _jumpAction = _player.actions.FindAction(Jump, true);
        _fireAction = _player.actions.FindAction(Fire, true);
    }
}

#endif