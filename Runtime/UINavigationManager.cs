using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UINavigationManager : MonoBehaviour
{
    public static UINavigationManager Instance { get; private set; }

    private Selectable[] _selectableCache;
    private Selectable _previousSelection;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!TryGetCurrentEventSystem(out EventSystem eventSystem))
        {
            return;
        }

        GameObject currentSelection = eventSystem.currentSelectedGameObject;
        if (currentSelection)
        {
            Selectable selectable = currentSelection.GetComponent<Selectable>();
            if (IsNavigable(selectable))
            {
                if (!_previousSelection || _previousSelection.gameObject != currentSelection)
                {
                    Debug.Log($"record selection: {selectable}", selectable);
                    _previousSelection = selectable;
                }
            }
            else
            {
                Debug.Log($"cancel selection for not navigable: {selectable}", selectable);
                eventSystem.SetSelectedGameObject(null);
            }
        }
    }

    public void EnsureSelection()
    {
        if (!TryGetCurrentEventSystem(out EventSystem eventSystem))
        {
            return;
        }

        if (TryGetSelectedComponent(out Selectable currentSelectable))
        {
            if (IsNavigable(currentSelectable))
            {
                // already selected, no need change
                return;
            }
        }

        int selectableCount = Selectable.allSelectableCount;
        if (_selectableCache == null || _selectableCache.Length < selectableCount)
        {
            _selectableCache = new Selectable[selectableCount];
        }

        selectableCount = Selectable.AllSelectablesNoAlloc(_selectableCache);
        if (selectableCount <= 0)
        {
            return;
        }

        if (IsNavigable(_previousSelection))
        {
            Debug.Log($"resume selection: {_previousSelection}", _previousSelection);
            eventSystem.SetSelectedGameObject(_previousSelection.gameObject);
        }
        else
        {
            Selectable firstSelectable = _selectableCache.Take(selectableCount).FirstOrDefault(IsNavigable);
            if (firstSelectable)
            {
                Debug.Log($"last selection can not be navigated. selection first navigable: {firstSelectable}", firstSelectable);
                eventSystem.SetSelectedGameObject(firstSelectable.gameObject);
            }
        }
    }

    public static bool IsNavigable(Object unityObject)
    {
        if (!unityObject)
        {
            return false;
        }

        // find selectable
        if (unityObject is Selectable selectable)
        {
            // ignore
        }
        else if (unityObject is GameObject gameObject)
        {
            selectable = gameObject.GetComponent<Selectable>();
        }
        else
        {
            return false;
        }

        return selectable
               && selectable.isActiveAndEnabled
               && selectable.IsInteractable()
               && selectable.navigation.mode != Navigation.Mode.None;
    }

    public static bool TryGetSelectedComponent<T>(out T component)
    {
        component = default;

        if (!TryGetCurrentEventSystem(out EventSystem eventSystem))
        {
            return false;
        }

        GameObject selectedGameObject = eventSystem.currentSelectedGameObject;
        return selectedGameObject && selectedGameObject.TryGetComponent(out component);
    }

    public static void PrintDebugInfo()
    {
        Selectable[] selectables = Selectable.allSelectablesArray.Where(IsNavigable).ToArray();
        foreach (Selectable selectable in selectables)
        {
            Debug.Log($"selection: {selectable}", selectable);
        }

        Debug.Log($"all selectable count: {selectables.Length}");

        if (!TryGetCurrentEventSystem(out EventSystem eventSystem))
        {
            return;
        }

        GameObject current = eventSystem.currentSelectedGameObject;
        Debug.Log($"current selected: {current}", current);
    }

    private static bool TryGetCurrentEventSystem(out EventSystem eventSystem)
    {
        eventSystem = EventSystem.current;
        if (!eventSystem)
        {
            Debug.LogWarning("current scene has not event system");
            return false;
        }

        return true;
    }
}