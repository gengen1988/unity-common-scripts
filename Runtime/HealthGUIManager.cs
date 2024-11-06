using System.Collections.Generic;
using UnityEngine;

public class HealthGUIManager : MonoBehaviour, IComponentManager<ActorHealth>
{
    public GameObject HealthBarPrefab;
    public GameObject DamageNumberPrefab;
    public RectTransform Container;
    public Vector2 DefaultHealthBarOffset = Vector2.up;

    private readonly HashSet<ActorHealth> _healthSubjects = new();
    private readonly Dictionary<ActorHealth, HealthBar> _widgetBySubject = new();

    private void Awake()
    {
        IComponentManager<ActorHealth>.RegisterManager(this);
    }

    private void OnDestroy()
    {
        IComponentManager<ActorHealth>.DeregisterManager(this);
    }

    public void OnComponentEnabled(ActorHealth component)
    {
        component.OnDamage += HandleDamage;
        _healthSubjects.Add(component);

        // create widgets
        GameObject newWidget = Instantiate(HealthBarPrefab, Container.transform);
        HealthBar healthBarImage = newWidget.GetComponent<HealthBar>();
        _widgetBySubject[component] = healthBarImage;
    }

    public void OnComponentDisabled(ActorHealth component)
    {
        component.OnDamage -= HandleDamage;
        _healthSubjects.Remove(component);

        // remove widgets
        HealthBar bar = _widgetBySubject[component];
        Destroy(bar.gameObject);
        _widgetBySubject.Remove(component);
    }

    // public void Refresh(HUDCtrl hud)
    // {
    //     // Update position and ratio
    //     Camera cam = hud.GetCamera();
    //     foreach (HealthSubject subject in _healthSubjects)
    //     {
    //         if (subject && _widgetBySubject.TryGetValue(subject, out HealthBarCtrl bar))
    //         {
    //             UpdateHealthBars(subject, bar, cam);
    //         }
    //     }
    // }

    private void UpdateHealthBars(ActorHealth subject, HealthBar bar, Camera cam)
    {
        Vector3 healthBarWorldPosition = subject.transform.position + (Vector3)DefaultHealthBarOffset;
        Vector3 screenPosition = cam.WorldToScreenPoint(healthBarWorldPosition);
        Transform barTrans = bar.transform;
        barTrans.position = screenPosition;
        float ratio = (float)subject.CurrentHP / subject.DefaultHP;
        bar.SetRatio(ratio);
        bar.SetAlpha(ratio < 1 ? 1 : 0);
    }

    private void HandleDamage(ActorOld actorOld, int damage)
    {
        Transform trans = actorOld.transform;
        GameObject damageNumberObject = PoolUtil.Spawn(DamageNumberPrefab, trans.position, trans.rotation);
        damageNumberObject.TryGetComponent(out DamageNumber damageNumber);
        damageNumber.Init(damage);
    }
}