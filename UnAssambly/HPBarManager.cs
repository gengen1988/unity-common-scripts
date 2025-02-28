using System.Collections.Generic;
using UnityEngine;

public class HPBarManager : MonoBehaviour
{
    [SerializeField] private GameObject HPBarPrefab;
    [SerializeField] private RectTransform Container;

    private readonly HashSet<Health> _healthSubjects = new();
    private readonly Dictionary<Health, GameObject> _widgetBySubject = new();

    private void OnEnable()
    {
        GlobalEventBus.Subscribe<OnActorReady>(HandleActorReady);
        GlobalEventBus.Subscribe<OnActorKill>(HandleActorKill);
    }

    private void OnDisable()
    {
        GlobalEventBus.Unsubscribe<OnActorReady>(HandleActorReady);
        GlobalEventBus.Unsubscribe<OnActorKill>(HandleActorKill);
    }

    private void LateUpdate()
    {
        // Update position and ratio
        foreach (var subject in _healthSubjects)
        {
            if (subject && _widgetBySubject.TryGetValue(subject, out var widget))
            {
                RefreshHealthBar(subject, widget);
            }
        }
    }

    private void HandleActorReady(OnActorReady evt)
    {
        // no health
        if (!evt.Actor.TryGetComponent(out Health health))
        {
            return;
        }

        // hide
        if (evt.Actor.TryGetComponent(out HPBarOverride config))
        {
            if (config.IsHide)
            {
                return;
            }
        }

        _healthSubjects.Add(health);

        // create widgets
        var newWidget = Instantiate(HPBarPrefab, Container);
        _widgetBySubject[health] = newWidget;
    }

    private void HandleActorKill(OnActorKill evt)
    {
        // no health
        if (!evt.Actor.TryGetComponent(out Health health))
        {
            return;
        }

        // hide
        if (evt.Actor.TryGetComponent(out HPBarOverride config))
        {
            if (config.IsHide)
            {
                return;
            }
        }

        _healthSubjects.Remove(health);

        // remove widgets
        var widget = _widgetBySubject[health];
        Destroy(widget);
        _widgetBySubject.Remove(health);
    }

    private void RefreshHealthBar(Health subject, GameObject widget)
    {
        // position
        var healthBarWorldPosition = subject.transform.position;
        var mainCam = CameraManager.Instance.MainCamera;
        var screenPosition = mainCam.WorldToScreenPoint(healthBarWorldPosition);
        var trans = widget.transform;
        trans.position = screenPosition;

        // ratio
        widget.TryGetComponent(out UIFillBar bar);
        bar.Ratio = subject.HealthRatio;

        // alpha
        widget.TryGetComponent(out CanvasGroup canvasGroup);
        canvasGroup.alpha = subject.HealthRatio < 1 ? 1 : 0;
    }
}