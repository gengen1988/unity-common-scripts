using System;
using UnityEngine;

[Obsolete]
public class DebugTimeScaleChange : MonoBehaviour
{
    public bool VariableTimeScale;
    public bool VariableFrameRate;

    private void Update()
    {
        if (VariableTimeScale)
        {
            Time.timeScale = Mathf.Cos(Time.unscaledTime) / 2f + .5f;
        }
        else
        {
            Time.timeScale = 1;
        }

        if (VariableFrameRate)
        {
            Application.targetFrameRate = Mathf.FloorToInt(60f + Mathf.Cos(Time.unscaledTime) * 30f);
        }
        else
        {
            Application.targetFrameRate = 60;
        }
    }
}