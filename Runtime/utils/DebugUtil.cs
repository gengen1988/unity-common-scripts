using UnityEngine;

public static class DebugUtil
{
    public static void DrawRectangle(Vector3 center, Vector3 size, Color color)
    {
        Vector3 topLeft = new Vector3(center.x - size.x / 2, center.y + size.y / 2);
        Vector3 topRight = new Vector3(center.x + size.x / 2, center.y + size.y / 2);
        Vector3 bottomLeft = new Vector3(center.x - size.x / 2, center.y - size.y / 2);
        Vector3 bottomRight = new Vector3(center.x + size.x / 2, center.y - size.y / 2);

        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);
    }

    public static void DrawCross(Vector3 center, Color color)
    {
        Debug.DrawLine(center - Vector3.right, center + Vector3.right, color);
        Debug.DrawLine(center - Vector3.up, center + Vector3.up, color);
    }
}