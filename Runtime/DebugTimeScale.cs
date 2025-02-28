using UnityEngine;

// for debug or inspect time related procedures
// do not use Time.timeScale implements gameplay 
public class DebugTimeScale : MonoBehaviour
{
    private void Update()
    {
        // increase
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            var targetScale = Time.timeScale * 2 / 1;
            ChangeTimeScale(targetScale);
        }
        // decrease
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            var targetScale = Time.timeScale * 1 / 2;
            ChangeTimeScale(targetScale);
        }
        // reset
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ChangeTimeScale(1);
        }
    }

    private void ChangeTimeScale(float value)
    {
        Time.timeScale = value;
        Debug.Log($"timescale set to: {value:0.00}x");
    }
}