using UnityEngine;

public static class DebugUtil
{
    public static void DebugDrawRectangle(Vector3 center, Vector3 size, Color rectColor)
    {
        Vector3 topLeft = new Vector3(center.x - size.x / 2, center.y + size.y / 2);
        Vector3 topRight = new Vector3(center.x + size.x / 2, center.y + size.y / 2);
        Vector3 bottomLeft = new Vector3(center.x - size.x / 2, center.y - size.y / 2);
        Vector3 bottomRight = new Vector3(center.x + size.x / 2, center.y - size.y / 2);

        Debug.DrawLine(topLeft, topRight, rectColor);
        Debug.DrawLine(topRight, bottomRight, rectColor);
        Debug.DrawLine(bottomRight, bottomLeft, rectColor);
        Debug.DrawLine(bottomLeft, topLeft, rectColor);
    }
}