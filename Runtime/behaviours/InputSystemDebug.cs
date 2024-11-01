using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputSystemDebug : MonoBehaviour
{
    public UnityEvent OnSpacePressedInThisFrame;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnSpacePressedInThisFrame.Invoke();
        }
    }
}