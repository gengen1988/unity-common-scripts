using FMODUnity;
using UnityEngine;

public class FeedbackFMOD : MonoBehaviour
{
    [SerializeField] private EventReference AudioEvent;

    private void Awake()
    {
        TryGetComponent(out Feedback feedback);
        feedback.OnPlay += HandlePlay;
    }

    private void HandlePlay(Feedback feedback)
    {
        RuntimeManager.PlayOneShot(AudioEvent);
    }
}