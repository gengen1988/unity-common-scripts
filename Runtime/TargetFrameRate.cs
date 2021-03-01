using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
    public int fps = 300;
    
    void Start()
    {
        Application.targetFrameRate = fps;
    }
}
