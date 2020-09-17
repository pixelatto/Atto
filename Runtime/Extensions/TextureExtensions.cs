using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureExtensions
{

    public static void SetPixels(this Texture2D texture, RectInt rect, Color[] colors)
    {
        texture.SetPixels(rect.x, rect.y, rect.width, rect.height, colors);
    }

    public static Color[] GetPixels(this Texture2D texture, RectInt rect, int mipLevel = 0)
    {
        return texture.GetPixels(rect.x, rect.y, rect.width, rect.height, mipLevel);
    }

    public static Color[] GetTileColors(this Texture2D texture, Vector2Int tileSize, int x, int y)
    {
        return texture.GetPixels(RectTools.GetTileRect(tileSize, x, y));
    }

    public static Sprite CreateSprite(this Texture2D texture, RectInt rect, Vector2? pivot = null, float pixelsPerUnit = 100)
    {
        if (pivot == null) { pivot = new Vector2(0.5f, 0.5f); };
        return Sprite.Create(texture, rect.ToRect(), (Vector2)pivot, pixelsPerUnit);
    }
}
