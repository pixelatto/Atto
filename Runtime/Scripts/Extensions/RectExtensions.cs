using UnityEngine;

public static class RectExtensions
{
    public static RectInt ToRectInt(this Rect rect)
    {
        return new RectInt(Mathf.RoundToInt(rect.x), Mathf.RoundToInt(rect.y), Mathf.RoundToInt(rect.width), Mathf.RoundToInt(rect.height));
    }

    public static Rect Move(this Rect rect, float x, float y)
    {
        return new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
    }

    public static Rect Move(this Rect rect, Vector2 offset)
    {
        return rect.Move(offset.x, offset.y);
    }

    public static Rect Offset(this Rect rect, float padding)
    {
        return new Rect(rect.x + padding, rect.y + padding, rect.width - 2 * padding, rect.height - 2 * padding);
    }

    public static Rect MoveUp(this Rect rect, float distance = -1)
    {
        if (distance == -1) { distance = rect.height; }
        return rect.Move(0, distance);
    }

    public static Rect MoveRight(this Rect rect, float distance = -1)
    {
        if (distance == -1) { distance = rect.width; }
        return rect.Move(distance, 0);
    }

    public static Rect MoveDown(this Rect rect, float distance = -1)
    {
        if (distance == -1) { distance = rect.height; }
        return rect.Move(0, -distance);
    }

    public static Rect MoveLeft(this Rect rect, float distance = -1)
    {
        if (distance == -1) { distance = rect.width; }
        return rect.Move(-distance, 0);
    }
}