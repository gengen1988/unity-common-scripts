using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class FeedbackCameraShake : MonoBehaviour
{
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _impulseSource = this.EnsureComponent<CinemachineImpulseSource>();
    }

    public void HandlePlay()
    {
        _impulseSource.GenerateImpulse();
    }
}