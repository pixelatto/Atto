using UnityEngine;
public static class RectIntExtensions
{
    public static Rect ToRect(this RectInt rect, float scale = 1)
    {
        return new Rect(rect.x * scale, rect.y * scale, rect.width * scale, rect.height * scale);
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

    public static RectInt ExpandBounds(this RectInt currentBox, Vector2Int newPoint)
    {
        // Si currentBounds es (0,0,0,0), entonces consideramos que newPoint es el primer punto
        if (currentBox.width == 0 && currentBox.height == 0)
        {
            return new RectInt(newPoint.x, newPoint.y, 1, 1);
        }

        // Calculamos los nuevos límites
        int xMin = Mathf.Min(currentBox.xMin, newPoint.x);
        int yMin = Mathf.Min(currentBox.yMin, newPoint.y);
        int xMax = Mathf.Max(currentBox.xMax, newPoint.x);
        int yMax = Mathf.Max(currentBox.yMax, newPoint.y);

        // Creamos un nuevo RectInt con las nuevas dimensiones
        return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
    }
}