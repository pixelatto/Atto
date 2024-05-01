using UnityEngine;

public static class Draw
{
    public static void Rect(this RectInt rect, Color color)
    {
        Rect(rect.ToRect(1), color);
    }

    public static void Rect(this Rect rect, Color color)
    {
        Gizmos.color = color;

        Vector3 topLeft = new Vector3(rect.xMin, rect.yMin, 0);
        Vector3 topRight = new Vector3(rect.xMax, rect.yMin, 0);
        Vector3 bottomLeft = new Vector3(rect.xMin, rect.yMax, 0);
        Vector3 bottomRight = new Vector3(rect.xMax, rect.yMax, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    public static void WireBox(Vector3 center, Vector3 halfExtents, Color color)
    {
        var v1 = center + new Vector3(+halfExtents.x, +halfExtents.y, +halfExtents.z);
        var v2 = center + new Vector3(-halfExtents.x, +halfExtents.y, +halfExtents.z);
        var v3 = center + new Vector3(-halfExtents.x, +halfExtents.y, -halfExtents.z);
        var v4 = center + new Vector3(+halfExtents.x, +halfExtents.y, -halfExtents.z);
        var v5 = center + new Vector3(+halfExtents.x, -halfExtents.y, +halfExtents.z);
        var v6 = center + new Vector3(-halfExtents.x, -halfExtents.y, +halfExtents.z);
        var v7 = center + new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
        var v8 = center + new Vector3(+halfExtents.x, -halfExtents.y, -halfExtents.z);

        Debug.DrawLine(v1, v2, color);
        Debug.DrawLine(v2, v3, color);
        Debug.DrawLine(v3, v4, color);
        Debug.DrawLine(v4, v1, color);

        Debug.DrawLine(v5, v6, color);
        Debug.DrawLine(v6, v7, color);
        Debug.DrawLine(v7, v8, color);
        Debug.DrawLine(v8, v5, color);

        Debug.DrawLine(v1, v5, color);
        Debug.DrawLine(v2, v6, color);
        Debug.DrawLine(v3, v7, color);
        Debug.DrawLine(v4, v8, color);
    }

    public static void Circle(Vector2 center, float radius, Color color, int resolution = 20)
    {
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
                Debug.DrawLine(currentPoint, prevPoint, color);
            }
            else
            {
                firstPoint = currentPoint;
            }
            prevPoint = currentPoint;
        }
        Debug.DrawLine(prevPoint, firstPoint, color);
    }

    public static void Arc(Vector2 center, float radius, Vector2 startDirection, Vector2 endDirection, Color color, bool clockwise = true, int resolution = 20)
    {

        float startAngle = Vector2.SignedAngle(Vector2.right, startDirection);
        float endAngle = Vector2.SignedAngle(Vector2.right, endDirection);
        Arc(center, radius, startAngle, endAngle, color, clockwise, resolution);
    }

    public static void Arc(Vector2 center, float radius, float startAngle, float endAngle, Color color, bool clockwise = true, int resolution = 20)
    {
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
                Debug.DrawLine(currentPoint, prevPoint, color);
            }
            prevPoint = currentPoint;
        }
        //Debug.Log("Start: " + startAngle + " End: "+ endAngle + " Delta: "+ deltaAngle);
        Debug.DrawLine(prevPoint, center + new Vector2(Mathf.Cos(endAngle), Mathf.Sin(endAngle)) * radius, color);
    }

    public static void Arrow(Vector2 origin, Vector2 target, Color color, float size = 0.1f)
    {
        var tangent = (target - origin).normalized;
        var normal = new Vector2(-tangent.y, tangent.x);
        //Debug.DrawLine(origin, target, color);
        Debug.DrawLine(origin - normal * size, target, color);
        Debug.DrawLine(origin + normal * size, target, color);
        Debug.DrawLine(origin + normal * size, origin - normal * size, color);
    }

    public static void Vector(Vector2 origin, Vector2 target, Color color, float size = 0.1f, bool closedArrowHead = true)
    {
        var tangent = (target - origin).normalized;
        var normal = new Vector2(-tangent.y, tangent.x);
        Debug.DrawLine(origin, closedArrowHead ? target - tangent * size : target, color);
        Debug.DrawLine(target, target - (tangent + normal) * size, color);
        Debug.DrawLine(target, target - (tangent - normal) * size, color);
        if (closedArrowHead)
        {
            Debug.DrawLine(target - (tangent + normal) * size, target - (tangent - normal) * size, color);
        }
    }
}