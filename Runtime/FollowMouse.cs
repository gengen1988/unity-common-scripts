using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private void Update()
    {
        transform.position = UnityUtil.GetMousePositionWorld();
    }
}