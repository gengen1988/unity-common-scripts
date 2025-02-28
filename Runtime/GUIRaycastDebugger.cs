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
            EventSystem eventSystem = EventSystem.current;
            PointerEventData evt = new PointerEventData(eventSystem);

            Vector3 mousePosition = Input.mousePosition;
            evt.position = mousePosition;

            Debug.Log($"debug raycast: {mousePosition}");
            List<RaycastResult> targets = new List<RaycastResult>();
            eventSystem.RaycastAll(evt, targets);

            IEnumerable<IGrouping<IEventSystemHandler, RaycastResult>> groups = from target in targets
                let go = target.gameObject
                let clickHandler = go.GetComponentInParent<IPointerClickHandler>() as IEventSystemHandler
                let downHandler = go.GetComponentInParent<IPointerDownHandler>() as IEventSystemHandler
                let handler = clickHandler ?? downHandler
                group target by handler
                into g
                select g;

            foreach (IGrouping<IEventSystemHandler, RaycastResult> group in groups)
            {
                IEventSystemHandler handler = group.Key;
                if (handler is Component component)
                {
                    GameObject handlerGameObject = component.gameObject;
                    Debug.Log($"handler: {handlerGameObject}", handlerGameObject);
                    foreach (RaycastResult target in group)
                    {
                        GameObject targetGameObject = target.gameObject;
                        Debug.Log($"- target: {targetGameObject}", targetGameObject);
                    }
                }
                else
                {
                    Debug.Log($"non handler");
                    foreach (RaycastResult target in group)
                    {
                        GameObject targetGameObject = target.gameObject;
                        Debug.Log($"- target: {targetGameObject}", targetGameObject);
                    }
                }
            }
        }
    }

    public void DebugLog(string text)
    {
        Debug.Log(text);
    }
}