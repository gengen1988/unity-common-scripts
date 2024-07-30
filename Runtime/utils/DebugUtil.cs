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

public static class GizmosUtil
{
    public static void DrawWireArc2D(Vector2 center, Vector2 from, float angle, float radius)
    {
        const int SEGMENTS = 24;
        float angleStep = angle / SEGMENTS;
        Vector2 actualFrom = from.normalized * radius;
        Vector2 previousPoint = center + actualFrom;
        for (int i = 1; i <= SEGMENTS; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angleStep * i);
            Vector2 currentPoint = center + (Vector2)(rotation * actualFrom);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    public static void DrawWireCircle2D(Vector2 center, float radius)
    {
        DrawWireArc2D(center, Vector2.right, 360f, radius);
    }
}