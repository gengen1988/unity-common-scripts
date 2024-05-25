using UnityEngine;

public class PositionProvider : MonoBehaviour
{
    public Vector2 Size = Vector2.one;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.2f);
        Gizmos.DrawCube(transform.position, Size);
    }

    public Vector3 GetRandomPosition()
    {
        return RandomUtil.PointInBox(transform.position, Size / 2);
    }
}