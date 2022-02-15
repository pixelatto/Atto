using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atto.Utils;

namespace Atto.Extensions
{
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
			return texture.GetPixels(RectTools.GetTileRect(tileSize, new Vector2Int(x, y)));
		}

		public static Sprite CreateSprite(this Texture2D texture, RectInt rect, Vector2? pivot = null, float pixelsPerUnit = 100)
		{
			if (pivot == null) { pivot = new Vector2(0.5f, 0.5f); };
			return Sprite.Create(texture, rect.ToRect(), (Vector2)pivot, pixelsPerUnit);
		}
		
        public static void BleedRectEdges(this Texture2D texture, RectInt paddedRect, int padding)
        {
            var leftBleedPixels = texture.GetPixels(new RectInt(paddedRect.x + padding, paddedRect.y, 1, paddedRect.height));
            for (int i = 0; i < padding; i++)
            {
                var bleedRect = new RectInt(paddedRect.x + i, paddedRect.y, 1, paddedRect.height);
                texture.SetPixels(bleedRect, leftBleedPixels);
            }

            var rightBleedPixels = texture.GetPixels(new RectInt(paddedRect.x + paddedRect.width - 1 - padding, paddedRect.y, 1, paddedRect.height));
            for (int i = 0; i < padding; i++)
            {
                var bleedRect = new RectInt(paddedRect.x + paddedRect.width - 1 - i, paddedRect.y, 1, paddedRect.height);
                texture.SetPixels(bleedRect, rightBleedPixels);
            }

            var bottomBleedPixels = texture.GetPixels(new RectInt(paddedRect.x, paddedRect.y + padding, paddedRect.width, 1));
            for (int i = 0; i < padding; i++)
            {
                var bleedRect = new RectInt(paddedRect.x, paddedRect.y + i, paddedRect.width, 1);
                texture.SetPixels(bleedRect, bottomBleedPixels);
            }

            var topBleedPixels = texture.GetPixels(new RectInt(paddedRect.x, paddedRect.y + paddedRect.height - 1 - padding, paddedRect.width, 1));
            for (int i = 0; i < padding; i++)
            {
                var bleedRect = new RectInt(paddedRect.x, paddedRect.y + paddedRect.height - 1 - i, paddedRect.width, 1);
                texture.SetPixels(bleedRect, topBleedPixels);
            }
        }
	}
}
