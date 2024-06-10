using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    const float UPDATE_INTERVAL = 0.2f;

    [Range(30f, 120f)]
    public int FPS = 60;

    private int _elapsedFrames;
    private float _elapsedTime;
    private float _actualFPS;

    private void OnGUI()
    {
        GUILayout.Label($"FPS: {_actualFPS:0.0}");
    }

    public void Update()
    {
        if (Application.targetFrameRate != FPS)
        {
            Application.targetFrameRate = FPS;
        }

        if (_elapsedTime >= UPDATE_INTERVAL)
        {
            _actualFPS = _elapsedFrames / _elapsedTime;
            _elapsedTime = 0;
            _elapsedFrames = 0;
        }

        _elapsedTime += Time.unscaledDeltaTime;
        _elapsedFrames++;
    }
}