using UnityEngine;

[DisallowMultipleComponent]
public class VFXCtrl : MonoBehaviour
{
    private float _timeoutTime;
    private float _elapsedTime;
    private bool _isFollowing;
    private int _followingSpawnStamp;
    private bool _particleSystemStopped;
    private Transform _selfTransform;
    private Transform _followingTransform;
    private ParticleSystem _ps;
    private ParticleSystemEventProxy _proxy;

    private void Awake()
    {
        _selfTransform = transform;
        _ps = GetComponentInChildren<ParticleSystem>();
        _proxy = _ps.gameObject.GetOrAddComponent<ParticleSystemEventProxy>();
        _proxy.OnStopped += HandleParticleSystemStopped;
    }

    private void OnDestroy()
    {
        if (_proxy)
        {
            _proxy.OnStopped -= HandleParticleSystemStopped;
        }
    }

    private void HandleParticleSystemStopped()
    {
        _particleSystemStopped = true;
    }

    public void Init(Transform following, float timeout)
    {
        if (following)
        {
            _isFollowing = true;
            _followingTransform = following;
            _followingSpawnStamp = PoolWrapper.GetStamp(following);
        }
        else
        {
            _isFollowing = false;
        }

        _timeoutTime = timeout;
        _elapsedTime = 0f;
        _particleSystemStopped = false;
    }

    public void Tick(float deltaTime)
    {
        // timeout logic
        if (_timeoutTime > 0)
        {
            // store time
            _elapsedTime += deltaTime;

            // stop emitting if timeout
            if (_elapsedTime > _timeoutTime)
            {
                Finish();
            }
        }

        // follow logic
        if (_isFollowing)
        {
            if (PoolWrapper.IsAlive(_followingTransform, _followingSpawnStamp))
            {
                _selfTransform.position = _followingTransform.position;
            }
            // else
            // {
            //     Debug.LogWarning($"following died but vfx remained: {this}", this);
            // }
        }
    }

    public bool IsFinished()
    {
        return _particleSystemStopped;
    }

    public void Finish()
    {
        if (_ps.isPlaying)
        {
            // Debug.Log($"stop particles: {this}");
            _ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}