using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    void Update()
    {
        transform.position = UnityUtil.GetMouseWorldPosition();
    }
}