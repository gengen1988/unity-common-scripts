using UnityEngine;

public static class DebugUtil
{
    public static void DrawRectangle(Vector3 center, Vector3 size, Color color)
    {
        var topLeft = new Vector3(center.x - size.x / 2, center.y + size.y / 2);
        var topRight = new Vector3(center.x + size.x / 2, center.y + size.y / 2);
        var bottomLeft = new Vector3(center.x - size.x / 2, center.y - size.y / 2);
        var bottomRight = new Vector3(center.x + size.x / 2, center.y - size.y / 2);

        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);
    }

    public static void DrawSquare(Vector3 center, float size, Color color)
    {
        var topLeft = new Vector3(center.x - size / 2, center.y + size / 2);
        var topRight = new Vector3(center.x + size / 2, center.y + size / 2);
        var bottomLeft = new Vector3(center.x - size / 2, center.y - size / 2);
        var bottomRight = new Vector3(center.x + size / 2, center.y - size / 2);

        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);
    }

    public static void DrawCross(Vector3 center, Color color)
    {
        Debug.DrawLine(center - Vector3.right - Vector3.up, center + Vector3.right + Vector3.up, color);
        Debug.DrawLine(center - Vector3.right + Vector3.up, center + Vector3.right - Vector3.up, color);
    }

    public static void DrawWireArc2D(Vector2 center, Vector2 from, float angle, float radius, Color color)
    {
        const int SEGMENTS = 24;
        var angleStep = angle / SEGMENTS;
        var actualFrom = from.normalized * radius;
        var previousPoint = center + actualFrom;

        // Debug.DrawLine(center, previousPoint, color);

        for (var i = 1; i <= SEGMENTS; i++)
        {
            var rotation = Quaternion.Euler(0, 0, angleStep * i);
            var currentPoint = center + (Vector2)(rotation * actualFrom);
            Debug.DrawLine(previousPoint, currentPoint, color);
            previousPoint = currentPoint;
        }

        // Debug.DrawLine(center, previousPoint, color);
    }

    public static void DrawWireCircle2D(Vector2 center, float radius, Color color)
    {
        DrawWireArc2D(center, Vector2.right, 360f, radius, color);
    }

    public static void CreatePositionIndicatorAndPause(Vector3 position)
    {
        var go = new GameObject("position indicator");
        go.transform.position = position;
        Debug.Break();
    }
}

public static class GizmosUtil
{
    public static void DrawWireArc2D(Vector2 center, Vector2 from, float angle, float radius)
    {
        const int SEGMENTS = 24;
        var angleStep = angle / SEGMENTS;
        var actualFrom = from.normalized * radius;
        var previousPoint = center + actualFrom;

        // Gizmos.DrawLine(center, previousPoint);

        for (var i = 1; i <= SEGMENTS; i++)
        {
            var rotation = Quaternion.Euler(0, 0, angleStep * i);
            var currentPoint = center + (Vector2)(rotation * actualFrom);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }


        // Gizmos.DrawLine(center, previousPoint);
    }

    public static void DrawWireCircle2D(Vector2 center, float radius)
    {
        DrawWireArc2D(center, Vector2.right, 360f, radius);
    }

    public static void DrawWireFan2D(Vector2 center, Vector2 from, float angle, float innerRadius, float outerRadius)
    {
        DrawWireArc2D(center, from, angle, outerRadius);
        DrawWireArc2D(center, from, angle, innerRadius);

        // Connect the inner and outer arcs at the endpoints
        var startRotation = Quaternion.identity;
        var endRotation = Quaternion.Euler(0, 0, angle);

        var innerStart = center + (Vector2)(startRotation * from.normalized * innerRadius);
        var outerStart = center + (Vector2)(startRotation * from.normalized * outerRadius);

        var innerEnd = center + (Vector2)(endRotation * from.normalized * innerRadius);
        var outerEnd = center + (Vector2)(endRotation * from.normalized * outerRadius);

        Gizmos.DrawLine(innerStart, outerStart);
        Gizmos.DrawLine(innerEnd, outerEnd);
    }

    // angle 0 is plus, angle 45 is multiply
    public static void DrawCross2D(Vector3 center, float angle = 0)
    {
        var rotation = Quaternion.Euler(0, 0, angle);
        var right = rotation * Vector3.right;
        var up = rotation * Vector3.up;
        Gizmos.DrawLine(center - right, center + right);
        Gizmos.DrawLine(center - up, center + up);
    }
}