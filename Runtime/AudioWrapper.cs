using FMODUnity;

public static class AudioWrapper
{
    public static void PlayOneShotIfExists(EventReference audioEvent)
    {
        if (!audioEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(audioEvent);
        }
    }
}