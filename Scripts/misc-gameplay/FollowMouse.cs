using UnityEngine;

[DefaultExecutionOrder(-1000)] // before other logic. such as cinemachine target group (when refresh on update)
public class FollowMouse : MonoBehaviour
{
    private void Update()
    {
        transform.position = UnityUtil.GetMousePositionWorld();
    }
}