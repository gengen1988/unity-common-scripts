#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

public class BrainInput : MonoBehaviour, IBrainDecide<UnitContext>
{
    private const float BUTTON_DEADZONE = .5f;
    private const float AXIS_DEADZONE = .1f;
    private const float PREVIOUS_WEIGHT = .8f;
    private const float MAX_INTERGRAL_MAGNITUDE = 2f;

    [SerializeField] private string move = "Player/Move";
    [SerializeField] private string look = "Player/Look";
    [SerializeField] private string attack = "Player/Attack";
    [SerializeField] private string jump = "Player/Jump";
    [SerializeField] private float mouseLookDeadZone = 0.5f;
    [field: SerializeField] public Transform mouseLookCenter { get; set; }

    private bool _initialized;
    private bool _jump;
    private bool _attack;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;
    private Vector2 _moveVector;
    private Vector2 _lookVector;

    private Camera _mainCam;
    private BrainManager<UnitContext> _manager;

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || !_initialized)
        {
            return;
        }

        var origin = transform.position;

        // Draw move vector
        if (_moveVector.magnitude > AXIS_DEADZONE)
        {
            Gizmos.color = Color.blue;
            var moveDirection = _moveVector.normalized;
            var moveScale = 1.0f;
            Gizmos.DrawLine(origin, origin + new Vector3(moveDirection.x, moveDirection.y, 0) * moveScale);
        }

        // Draw look vector
        if (_lookVector.magnitude > AXIS_DEADZONE)
        {
            Gizmos.color = Color.red;
            var lookDirection = _lookVector.normalized;
            var lookScale = 1.5f;
            Gizmos.DrawLine(origin, origin + new Vector3(lookDirection.x, lookDirection.y, 0) * lookScale);
        }
    }

    private void Reset()
    {
        mouseLookCenter = transform;
    }

    private void Update()
    {
        if (!_initialized)
        {
            Initialize();
        }

        if (!_initialized)
        {
            return;
        }

        // move vector
        var prevMoveVector = _moveVector;
        var currentMoveVector = _moveAction.ReadValue<Vector2>();
        var nextMoveVector = prevMoveVector * PREVIOUS_WEIGHT + currentMoveVector;
        _moveVector = Vector3.ClampMagnitude(nextMoveVector, MAX_INTERGRAL_MAGNITUDE);

        // mouse look
        var currentMouse = Mouse.current;
        if (mouseLookCenter && currentMouse != null)
        {
            // try to find main camera
            if (!_mainCam)
            {
                _mainCam = Camera.main;
            }

            _lookVector = MouseLook(currentMouse, _mainCam);
        }
        // gamepad look
        else
        {
            _lookVector = _lookAction.ReadValue<Vector2>();
        }

        // cache until actor tick
        if (_jumpAction.ReadValue<float>() > BUTTON_DEADZONE)
        {
            _jump = true;
        }

        if (_attackAction.ReadValue<float>() > BUTTON_DEADZONE)
        {
            _attack = true;
        }
    }

    private Vector2 MouseLook(Mouse mouse, Camera cam)
    {
        if (!cam)
        {
            Debug.LogAssertion("missing camera for mouse look", this);
        }

        if (!cam.orthographic)
        {
            Debug.LogAssertion("Camera is not orthographic, mouse look invalid", cam);
        }

        var mousePositionScreen = mouse.position.ReadValue();
        var mousePositionWorld = (Vector2)cam.ScreenToWorldPoint(mousePositionScreen);
        var center = (Vector2)mouseLookCenter.position;
        var los = mousePositionWorld - center;
        if (los.sqrMagnitude < mouseLookDeadZone * mouseLookDeadZone)
        {
            return Vector2.zero;
        }

        return los;
    }

    private void Initialize()
    {
        _initialized = false;

        var input = PlayerInput.GetPlayerByIndex(0);
        if (!input)
        {
            // Debug.LogWarning("PlayerInput not ready");
            return;
        }

        _moveAction = input.actions.FindAction(move);
        _lookAction = input.actions.FindAction(look);
        _jumpAction = input.actions.FindAction(jump);
        _attackAction = input.actions.FindAction(attack);
        _initialized = true;
    }

    public void Decide(UnitContext context)
    {
        if (!_initialized)
        {
            return;
        }

        // move
        if (_moveVector.magnitude < AXIS_DEADZONE)
        {
            _moveVector = Vector2.zero;
        }

        context.MoveIntent = _moveVector;
        context.LookIntent = _lookVector;
        context.RequestJump = _jump;
        context.RequestAttack = _attack;

        // reset
        _attack = false;
        _jump = false;
    }
}

#endif