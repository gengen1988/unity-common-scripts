using Sirenix.OdinInspector;
using UnityEngine;

public class UIDamageNumberManager : MonoBehaviour
{
    [SerializeField] private DamageNumberCtrl prefab;
    [SerializeField] private RectTransform container;

    private EventBinding<OnHealthTakeDamage> _bindingHealthTakeDamage;
    private Camera _camera;

    private void Awake()
    {
        _bindingHealthTakeDamage = new EventBinding<OnHealthTakeDamage>(HandleHealthTakeDamage);
    }

    private void OnDestroy()
    {
        _bindingHealthTakeDamage.Dispose();
    }


    private void OnEnable()
    {
        _camera = Camera.main;
        GlobalEventBus<OnHealthTakeDamage>.Register(_bindingHealthTakeDamage);
    }

    private void OnDisable()
    {
        GlobalEventBus<OnHealthTakeDamage>.Deregister(_bindingHealthTakeDamage);
    }

    private void HandleHealthTakeDamage(OnHealthTakeDamage evt)
    {
        var health = evt.Subject;
        var position = health.transform.position;

        // world position to screen position
        var screenPoint = _camera.WorldToScreenPoint(position);

        var instance = PoolUtil.Spawn(prefab, screenPoint, Quaternion.identity, container);
        instance.Show(evt.DamageAmount);
    }

    [Button]
    private void DebugEvent(Health subject)
    {
        GlobalEventBus<OnHealthTakeDamage>.Raise(new OnHealthTakeDamage
        {
            Subject = subject,
            DamageAmount = 19000,
        });
    }
}