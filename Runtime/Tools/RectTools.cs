using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atto.Utils
{
	public static class RectTools
	{
		public static RectInt GetTileRect(Vector2Int tileSize, int x, int y)
		{
			return new RectInt(tileSize.x * x, tileSize.y * y, tileSize.x, tileSize.y);
		}
	}
}

