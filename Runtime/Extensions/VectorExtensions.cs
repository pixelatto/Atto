using UnityEngine;

namespace Atto.Extensions
{
	public static class VectorExtensions
	{

		public static Vector2 Project(this Vector2 vector, Vector2 projectionDirection)
		{
			return Vector3.Project(vector, projectionDirection);
		}

		public static Vector2 LeftPerpendicular(this Vector2 vector)
		{
			return new Vector2(-vector.y, vector.x);
		}

		public static Vector2 RightPerpendicular(this Vector2 vector)
		{
			return new Vector2(vector.y, -vector.x);
		}

	}
}