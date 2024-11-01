using UnityEngine;

public class Buff : MonoBehaviour
{
    public event ActorEvent OnAdd;

    private BuffProfile _profile;
    private ActorBuffManager _manager;
    private int _stackCount;

    public Actor Owner => _manager.Owner;

    private void OnDestroy()
    {
        OnAdd = null;
    }

    public void Init(ActorBuffManager manager, BuffProfile profile)
    {
        _manager = manager;
        _profile = profile;
        _stackCount = 0;
    }

    public void Add()
    {
        if (_stackCount < _profile.StackCapacity)
        {
            _stackCount++;
        }

        OnAdd?.Invoke(Owner);
    }

    public void Kill()
    {
        _manager.RemoveBuff(_profile);
    }

    public int GetStackCount()
    {
        return _stackCount;
    }
}