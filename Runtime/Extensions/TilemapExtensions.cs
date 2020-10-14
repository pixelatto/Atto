using System;
using UnityEngine.Tilemaps;

namespace Atto.Extensions
{
	public static class TilemapExtensions
	{

		public static TileBase[] GetUsedTiles(this Tilemap tilemap, int maxArraySize = 100)
		{
			var tiles = new TileBase[tilemap.GetUsedTilesCount()];
			tilemap.GetUsedTilesNonAlloc(tiles);

			return tiles;
		}
	}
}