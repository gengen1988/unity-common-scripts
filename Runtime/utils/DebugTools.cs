﻿using UnityEngine;

public static class DebugTools
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

    public static void DrawSquare(Vector3 center, float size, Color color)
    {
        Vector3 topLeft = new Vector3(center.x - size / 2, center.y + size / 2);
        Vector3 topRight = new Vector3(center.x + size / 2, center.y + size / 2);
        Vector3 bottomLeft = new Vector3(center.x - size / 2, center.y - size / 2);
        Vector3 bottomRight = new Vector3(center.x + size / 2, center.y - size / 2);

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
        float angleStep = angle / SEGMENTS;
        Vector2 actualFrom = from.normalized * radius;
        Vector2 previousPoint = center + actualFrom;

        Debug.DrawLine(center, previousPoint, color);

        for (int i = 1; i <= SEGMENTS; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, angleStep * i);
            Vector2 currentPoint = center + (Vector2)(rotation * actualFrom);
            Debug.DrawLine(previousPoint, currentPoint, color);
            previousPoint = currentPoint;
        }

        Debug.DrawLine(center, previousPoint, color);
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

public static class GizmoTools
{
    public static void DrawWireArc2D(Vector2 center, Vector2 from, float angle, float radius)
    {
        const int SEGMENTS = 24;
        var angleStep = angle / SEGMENTS;
        var actualFrom = from.normalized * radius;
        var previousPoint = center + actualFrom;

        Gizmos.DrawLine(center, previousPoint);

        for (var i = 1; i <= SEGMENTS; i++)
        {
            var rotation = Quaternion.Euler(0, 0, angleStep * i);
            var currentPoint = center + (Vector2)(rotation * actualFrom);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        Gizmos.DrawLine(center, previousPoint);
    }

    public static void DrawWireCircle2D(Vector2 center, float radius)
    {
        DrawWireArc2D(center, Vector2.right, 360f, radius);
    }

    public static void DrawCross(Vector3 center)
    {
        Gizmos.DrawLine(center - Vector3.right - Vector3.up, center + Vector3.right + Vector3.up);
        Gizmos.DrawLine(center - Vector3.right + Vector3.up, center + Vector3.right - Vector3.up);
    }
}