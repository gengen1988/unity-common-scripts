public class Brain : Submodule<Brain>
{
    private bool _isQuitting;

    private void Start()
    {
        var actor = GetComponentInParent<Actor>();
        Mount(actor.gameObject);
    }

    private void OnDestroy()
    {
        if (_isQuitting)
        {
            return;
        }

        Unmount();
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}