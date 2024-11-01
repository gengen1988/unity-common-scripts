using UnityEngine;

/**
 * 作为标志和事件触发器
 */
public class HurtSubject : MonoBehaviour
{
    public int Priority; // 一个 hit subject 在同一帧命中多个 hurt subject 时，决定优先命中哪个。越小越先命中

    private IHurtHandler[] _handlers;

    private void Awake()
    {
        _handlers = this.GetAttachedComponents<IHurtHandler>();
    }

    public void NotifyHurtEvent(HitSubject hitSubject, HurtSubject hurtSubject, HitEventData evtData)
    {
        foreach (IHurtHandler handler in _handlers)
        {
            if (handler is not Behaviour unityBehaviour)
            {
                continue;
            }

            if (!unityBehaviour.isActiveAndEnabled)
            {
                continue;
            }

            handler.OnHurt(hitSubject, hurtSubject, evtData);
        }
    }
}