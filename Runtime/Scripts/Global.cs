using UnityEngine;

public class Global
{
    public static float pixelsPerUnit = 8;
    public static Vector2Int roomPixelSize = new Vector2Int(128, 72);
    public static Vector2Int resolution = new Vector2Int(128, 72);
    public static float aspectRatio => resolution.x / resolution.y;

    public static Vector2 roomWorldSize => new Vector2(128 / pixelsPerUnit, 72 / pixelsPerUnit);
    public float worldPixelSize => 1f / (float)pixelsPerUnit;
}