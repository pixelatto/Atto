using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atto.Utils
{
	public static class RectTools
	{
		public static RectInt GetTileRect(Vector2Int tileSize, Vector2Int tilePosition)
		{
			return new RectInt(tileSize.x * tilePosition.x, tileSize.y * tilePosition.y, tileSize.x, tileSize.y);
		}
	}
}

