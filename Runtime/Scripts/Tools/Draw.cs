﻿using System;
using UnityEngine;

public static class Draw
{
    public static void Rect(this RectInt rect, Color? color = null)
    {
        Rect(rect.ToRect(1), color);
    }

    public static void Rect(this Rect rect, Color? color = null)
    {
        if (color == null) { color = Color.white; }

        Vector3 topLeft = new Vector3(rect.xMin, rect.yMin, 0);
        Vector3 topRight = new Vector3(rect.xMax, rect.yMin, 0);
        Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMax, 0);
        Vector3 bottomRight = new Vector3(rect.xMax, rect.yMax, 0);

        Debug.DrawLine(topLeft, topRight, (Color)color);
        Debug.DrawLine(topRight, bottomRight, (Color)color);
        Debug.DrawLine(bottomRight, bottomLeft, (Color)color);
        Debug.DrawLine(bottomLeft, topLeft, (Color)color);
    }

    public static void WireBox(Vector3 center, Vector3 halfExtents, Color? color = null)
    {
        if (color == null) { color = Color.white; }

        var v1 = center + new Vector3(+halfExtents.x, +halfExtents.y, +halfExtents.z);
        var v2 = center + new Vector3(-halfExtents.x, +halfExtents.y, +halfExtents.z);
        var v3 = center + new Vector3(-halfExtents.x, +halfExtents.y, -halfExtents.z);
        var v4 = center + new Vector3(+halfExtents.x, +halfExtents.y, -halfExtents.z);
        var v5 = center + new Vector3(+halfExtents.x, -halfExtents.y, +halfExtents.z);
        var v6 = center + new Vector3(-halfExtents.x, -halfExtents.y, +halfExtents.z);
        var v7 = center + new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
        var v8 = center + new Vector3(+halfExtents.x, -halfExtents.y, -halfExtents.z);

        Debug.DrawLine(v1, v2, (Color)color);
        Debug.DrawLine(v2, v3, (Color)color);
        Debug.DrawLine(v3, v4, (Color)color);
        Debug.DrawLine(v4, v1, (Color)color);

        Debug.DrawLine(v5, v6, (Color)color);
        Debug.DrawLine(v6, v7, (Color)color);
        Debug.DrawLine(v7, v8, (Color)color);
        Debug.DrawLine(v8, v5, (Color)color);

        Debug.DrawLine(v1, v5, (Color)color);
        Debug.DrawLine(v2, v6, (Color)color);
        Debug.DrawLine(v3, v7, (Color)color);
        Debug.DrawLine(v4, v8, (Color)color);
    }

    public static void Circle(Vector2 center, float radius, Color? color = null, int resolution = 20)
    {
        if (color == null) { color = Color.white; }
        Vector2 firstPoint = Vector2.zero;
        Vector2 currentPoint;
        Vector2 prevPoint = Vector2.zero;
        var deltaAngle = (360f / (float)resolution) * Mathf.Deg2Rad;
        for (float angle = 0f; angle < 360f * Mathf.Deg2Rad; angle += deltaAngle)
        {
            var delta = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            currentPoint = center + delta;
            if (angle != 0f)
            {
                Debug.DrawLine(currentPoint, prevPoint, (Color)color);
            }
            else
            {
                firstPoint = currentPoint;
            }
            prevPoint = currentPoint;
        }
        Debug.DrawLine(prevPoint, firstPoint, (Color)color);
    }

    public static void Arc(Vector2 center, float radius, Vector2 startDirection, Vector2 endDirection, Color? color = null, bool clockwise = true, int resolution = 20)
    {
        if (color == null) { color = Color.white; }
        float startAngle = Vector2.SignedAngle(Vector2.right, startDirection);
        float endAngle = Vector2.SignedAngle(Vector2.right, endDirection);
        Arc(center, radius, startAngle, endAngle, color, clockwise, resolution);
    }

    public static void Arc(Vector2 center, float radius, float startAngle, float endAngle, Color? color = null, bool clockwise = true, int resolution = 20)
    {
        if (color == null) { color = Color.white; }
        Vector2 currentPoint;
        Vector2 prevPoint = Vector2.zero;

        if (clockwise)
        {
            int i = 0;
            while (endAngle < startAngle && i < 1000)
            {
                endAngle += 360f;
                i++;
            }
        }

        startAngle *= Mathf.Deg2Rad;
        endAngle *= Mathf.Deg2Rad;

        var deltaAngle = ((endAngle - startAngle) / (float)resolution);

        for (float angle = startAngle; clockwise ? angle < endAngle : angle > endAngle; angle += deltaAngle)
        {
            var delta = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            currentPoint = center + delta;
            if (angle != startAngle)
            {
                Debug.DrawLine(currentPoint, prevPoint, (Color)color);
            }
            prevPoint = currentPoint;
        }
        Debug.DrawLine(prevPoint, center + new Vector2(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * radius, (Color)color);
    }

    public static void ArrowHead(Vector2 origin, Vector2 target, Color? color = null, float size = 0.1f)
    {
        if (color == null) { color = Color.cyan; }
        var tangent = (target - origin).normalized;
        var normal = new Vector2(-tangent.y, tangent.x);

        Debug.DrawLine(origin - normal * size, target, (Color)color);
        Debug.DrawLine(origin + normal * size, target, (Color)color);
        Debug.DrawLine(origin + normal * size, origin - normal * size, (Color)color);
    }

    public static void Vector(Vector2 origin, Vector2 target, Color? color = null, float size = 0.1f, bool closedArrowHead = true)
    {
        if (color == null) { color = Color.cyan; }
        var tangent = (target - origin).normalized;
        var normal = new Vector2(-tangent.y, tangent.x);
        Debug.DrawLine(origin, closedArrowHead ? target - tangent * size : target, (Color)color);
        Debug.DrawLine(target, target - (tangent + normal) * size, (Color)color);
        Debug.DrawLine(target, target - (tangent - normal) * size, (Color)color);
        if (closedArrowHead)
        {
            Debug.DrawLine(target - (tangent + normal) * size, target - (tangent - normal) * size, (Color)color);
        }
    }

    public static void Cross(Vector2 position, Color? color = null, float size = 0.1f)
    {
        if (color == null) { color = Color.red; }
        Debug.DrawLine(position + Vector2.one * size, position - Vector2.one * size, (Color)color);
        Debug.DrawLine(position + Vector2.one.LeftPerpendicular() * size, position - Vector2.one.LeftPerpendicular() * size, (Color)color);
    }

    public static void PixelPrecise(Vector3 position, Color? color = null)
    {
        if (color == null) { color = Color.white; }
        var pixelPosition = new Vector2(Mathf.Floor(position.x * Global.pixelsPerUnit)/Global.pixelsPerUnit, Mathf.Floor(position.y * Global.pixelsPerUnit) / Global.pixelsPerUnit);
        var pixelRect = new Rect(pixelPosition.x, pixelPosition.y, 1f / Global.pixelsPerUnit, 1f / Global.pixelsPerUnit);
        Rect(pixelRect, color);
        Debug.DrawLine(pixelPosition, position, (Color)color);
        Debug.DrawLine(pixelPosition + Vector2.up    * 1f / Global.pixelsPerUnit, position, (Color)color);
        Debug.DrawLine(pixelPosition + Vector2.right * 1f / Global.pixelsPerUnit, position, (Color)color);
        Debug.DrawLine(pixelPosition + Vector2.one   * 1f / Global.pixelsPerUnit, position, (Color)color);
    }

    public static void Pixel(Vector3 position, Color? color = null)
    {
        if (color == null) { color = Color.white; }
        var pixelPosition = new Vector2(Mathf.Floor(position.x * Global.pixelsPerUnit) / Global.pixelsPerUnit, Mathf.Floor(position.y * Global.pixelsPerUnit) / Global.pixelsPerUnit);
        var pixelRect = new Rect(pixelPosition.x, pixelPosition.y, 1f / Global.pixelsPerUnit, 1f / Global.pixelsPerUnit);
        Rect(pixelRect, color);
    }

    public static void PixelSmall(Vector3 position, Color? color = null)
    {
        if (color == null) { color = Color.white; }
        var pixelPosition = new Vector2(Mathf.Floor(position.x * Global.pixelsPerUnit) / Global.pixelsPerUnit, Mathf.Floor(position.y * Global.pixelsPerUnit) / Global.pixelsPerUnit);
        var pixelRect = new Rect(pixelPosition.x, pixelPosition.y, 1f / Global.pixelsPerUnit, 1f / Global.pixelsPerUnit);
        Rect(pixelRect.Shrink(1f/Global.pixelsPerUnit * 0.1f), color);
    }
}