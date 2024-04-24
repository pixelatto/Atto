using UnityEngine;
public static class RectIntExtensions
{
    public static Rect ToRect(this RectInt rect)
    {
        return new Rect(rect.x, rect.y, rect.width, rect.height);
    }

    public static RectInt Move(this RectInt rect, int x, int y)
    {
        return new RectInt(rect.x + x, rect.y + y, rect.width, rect.height);
    }

    public static RectInt Move(this RectInt rect, Vector2Int offset)
    {
        return rect.Move(offset.x, offset.y);
    }

    public static RectInt Offset(this RectInt rect, int padding)
    {
        return new RectInt(rect.x + padding, rect.y + padding, rect.width - 2 * padding, rect.height - 2 * padding);
    }

    public static RectInt MoveUp(this RectInt rect, int distance = -1)
    {
        if (distance == -1) { distance = rect.height; }
        return rect.Move(0, distance);
    }

    public static RectInt MoveRight(this RectInt rect, int distance = -1)
    {
        if (distance == -1) { distance = rect.width; }
        return rect.Move(distance, 0);
    }

    public static RectInt MoveDown(this RectInt rect, int distance = -1)
    {
        if (distance == -1) { distance = rect.height; }
        return rect.Move(0, -distance);
    }

    public static RectInt MoveLeft(this RectInt rect, int distance = -1)
    {
        if (distance == -1) { distance = rect.width; }
        return rect.Move(-distance, 0);
    }
}