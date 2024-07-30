using FMODUnity;

public static class AudioWrapper
{
    public static void PlayOneShot(EventReference audioEvent)
    {
        if (!audioEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(audioEvent);
        }
    }
}