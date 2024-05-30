using System;
using UnityEngine;
using UnityEngine.Events;

[Obsolete]
[DefaultExecutionOrder(-100)] // debug input update first
public class DebugInputDeprecated : MonoBehaviour
{
    public bool TriggerEventsInFixedUpdate;
    public float MouseDeadZone;
    public Transform MouseCenter;

    public UnityEvent OnJump;

    private bool _jumpPressed;

    public Vector2 LeftStick { get; private set; }
    public Vector2 RightStick { get; private set; }

    public bool Fire1 { get; private set; }
    public bool Fire2 { get; private set; }
    public bool Fire3 { get; private set; }
    public bool Jump { get; private set; }

    private void Reset()
    {
        MouseDeadZone = 1f;
        MouseCenter = transform;
    }

    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var mouseWorldPoint = UnityUtil.GetMouseWorldPosition();
        var mouseVector = mouseWorldPoint - (Vector2)MouseCenter.position;

        LeftStick = new Vector2(horizontal, vertical).normalized;
        RightStick = mouseVector.magnitude < MouseDeadZone
            ? Vector2.zero
            : mouseVector.normalized;

        Fire1 = Input.GetButton("Fire1");
        Fire2 = Input.GetButton("Fire2");
        Fire3 = Input.GetButton("Fire3");
        Jump = Input.GetButton("Jump");

        if (Input.GetButtonDown("Jump"))
        {
            if (TriggerEventsInFixedUpdate)
            {
                _jumpPressed = true;
            }
            else
            {
                OnJump.Invoke();
            }
        }
    }

    private void FixedUpdate()
    {
        if (_jumpPressed)
        {
            OnJump.Invoke();
            _jumpPressed = false;
        }
    }
}