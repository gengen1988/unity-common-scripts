using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// hp bar in ui canvas
/// </summary>
public class HealthBarManager : MonoBehaviour
{
    [SerializeField] private GameObject BarPrefab;
    [SerializeField] private RectTransform Container;

    private readonly HashSet<Health> _healthSubjects = new();
    private readonly Dictionary<Health, GameObject> _widgetBySubject = new();

    private EventBinding<OnHealthEnabled> _onHealthEnabled;
    private EventBinding<OnHealthDisabled> _onHealthDisabled;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        _onHealthEnabled = new EventBinding<OnHealthEnabled>(HandleHealthEnabled);
        _onHealthDisabled = new EventBinding<OnHealthDisabled>(HandleHealthDisabled);
    }

    private void OnDestroy()
    {
        _onHealthEnabled.Dispose();
        _onHealthDisabled.Dispose();
    }

    private void OnEnable()
    {
        GlobalEventBus<OnHealthEnabled>.Register(_onHealthEnabled);
        GlobalEventBus<OnHealthDisabled>.Register(_onHealthDisabled);
    }

    private void OnDisable()
    {
        GlobalEventBus<OnHealthEnabled>.Deregister(_onHealthEnabled);
        GlobalEventBus<OnHealthDisabled>.Deregister(_onHealthDisabled);
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

    private void HandleHealthEnabled(OnHealthEnabled evt)
    {
        // // hide
        // if (evt.Actor.TryGetComponent(out HPBarOverride config))
        // {
        //     if (config.IsHide)
        //     {
        //         return;
        //     }
        // }
        //
        // _healthSubjects.Add(health);
        //
        // // create widgets
        // var newWidget = Instantiate(BarPrefab, Container);
        // _widgetBySubject[health] = newWidget;
    }

    private void HandleHealthDisabled(OnHealthDisabled evt)
    {
        // // hide
        // if (evt.Actor.TryGetComponent(out HPBarOverride config))
        // {
        //     if (config.IsHide)
        //     {
        //         return;
        //     }
        // }
        //
        // _healthSubjects.Remove(health);
        //
        // // remove widgets
        // var widget = _widgetBySubject[health];
        // Destroy(widget);
        // _widgetBySubject.Remove(health);
    }

    private void RefreshHealthBar(Health subject, GameObject widget)
    {
        // position
        var healthBarWorldPosition = subject.transform.position;
        var screenPosition = _camera.WorldToScreenPoint(healthBarWorldPosition);
        var trans = widget.transform;
        trans.position = screenPosition;

        // ratio
        widget.TryGetComponent(out UIFillBar bar);
        bar.Ratio = subject.HPRatio;

        // alpha
        widget.TryGetComponent(out CanvasGroup canvasGroup);
        canvasGroup.alpha = subject.HPRatio < 1 ? 1 : 0;
    }
}