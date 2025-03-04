#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

public class BrainInput : MonoBehaviour, ISubmoduleMount<Brain>
{
    private const float BUTTON_TRIGGER_DEADZONE = .5f;

    [SerializeField] private string move = "Player/Move";
    [SerializeField] private string look = "Player/Look";
    [SerializeField] private string fire = "Player/Fire";
    [SerializeField] private string jump = "Player/Jump";
    [SerializeField] private Transform mouseLookCenter;
    [SerializeField] private float mouseLookDeadZone = 0.5f;

    private bool _initialized;
    private bool _jump;
    private bool _fire;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _fireAction;
    private Actor _owner;
    private Pawn _pawn;
    private Camera _mainCam;

    private void Reset()
    {
        mouseLookCenter = transform;
    }

    private void Update()
    {
        if (!_initialized && !TryInitialize())
        {
            return;
        }

        // cache until actor tick
        if (_jumpAction.ReadValue<float>() > BUTTON_TRIGGER_DEADZONE)
        {
            _jump = true;
        }

        if (_fireAction.ReadValue<float>() > BUTTON_TRIGGER_DEADZONE)
        {
            _fire = true;
        }
    }

    private bool TryInitialize()
    {
        _initialized = false;
        var playerManager = PlayerManager.Instance;
        if (!playerManager)
        {
            // Debug.LogWarning("PlayerManager not ready");
            return false;
        }

        var input = playerManager.CurrentInput;
        if (!input)
        {
            // Debug.LogWarning("PlayerInput not ready");
            return false;
        }

        _moveAction = input.actions.FindAction(move);
        _lookAction = input.actions.FindAction(look);
        _jumpAction = input.actions.FindAction(jump);
        _fireAction = input.actions.FindAction(fire);
        _mainCam = Camera.main;
        _initialized = true;
        return true;
    }

    public void Mount(Brain submodule)
    {
        // bind pawn
        _pawn = submodule.Owner.EnsureComponent<Pawn>();
        _pawn.OnQuery += HandleQuery;

        // trigger initialize
        TryInitialize();
    }

    private void HandleQuery()
    {
        if (!_initialized)
        {
            return;
        }

        // move
        _pawn.SetVector2(IntentKey.Move, _moveAction.ReadValue<Vector2>());

        // mouse look
        var lookValue = Vector2.zero;
        var currentMouse = Mouse.current;
        if (mouseLookCenter && _mainCam && currentMouse != null)
        {
            var mousePosition = currentMouse.position.ReadValue();
            // Debug.Log($"mouse position: {mousePosition}");

            if (!_mainCam.orthographic)
            {
                Debug.LogAssertion("MainCamera is not orthographic, mouse look invalid", this);
            }

            var mousePositionWorld = (Vector2)_mainCam.ScreenToWorldPoint(mousePosition);
            var center = (Vector2)mouseLookCenter.position;
            // Debug.Log($"mouse position world: {mousePositionWorld}, {center}");

            var los = mousePositionWorld - center;
            if (los.sqrMagnitude > mouseLookDeadZone * mouseLookDeadZone)
            {
                lookValue = los;
            }
        }
        // gamepad look
        else
        {
            lookValue = _lookAction.ReadValue<Vector2>();
        }

        _pawn.SetVector2(IntentKey.Look, lookValue);

        // fire
        if (_fire)
        {
            _pawn.SetBool(IntentKey.Fire, true);
            _fire = false;
        }

        // jump
        if (_jump)
        {
            _pawn.SetBool(IntentKey.Jump, true);
            _jump = false;
        }
    }
}

#endif