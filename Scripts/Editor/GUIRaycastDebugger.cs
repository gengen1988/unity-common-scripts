using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUIRaycastDebugger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            var eventSystem = EventSystem.current;
            var evt = new PointerEventData(eventSystem);

            var mousePosition = Input.mousePosition;
            evt.position = mousePosition;

            Debug.Log($"debug raycast: {mousePosition}");
            var targets = new List<RaycastResult>();
            eventSystem.RaycastAll(evt, targets);

            var groups = from target in targets
                let go = target.gameObject
                let clickHandler = go.GetComponentInParent<IPointerClickHandler>() as IEventSystemHandler
                let downHandler = go.GetComponentInParent<IPointerDownHandler>() as IEventSystemHandler
                let handler = clickHandler ?? downHandler
                group target by handler
                into g
                select g;

            foreach (var group in groups)
            {
                var handler = group.Key;
                if (handler is Component component)
                {
                    var handlerGameObject = component.gameObject;
                    Debug.Log($"handler: {handlerGameObject}", handlerGameObject);
                    foreach (var target in group)
                    {
                        var targetGameObject = target.gameObject;
                        Debug.Log($"- target: {targetGameObject}", targetGameObject);
                    }
                }
                else
                {
                    Debug.Log($"non handler");
                    foreach (var target in group)
                    {
                        var targetGameObject = target.gameObject;
                        Debug.Log($"- target: {targetGameObject}", targetGameObject);
                    }
                }
            }
        }
    }
}