using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atto.Serialization.Converters
{

	public class VectorIntConverter<T> : IntListConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(T);
		}
	}

	public class Vector2IntMonolineConverter : VectorIntConverter<Vector2Int>
	{
		protected override List<int> ObjectToList(object value)
		{
			var vector = (Vector2Int)value;
			return new List<int>() { vector.x, vector.y };
		}

		protected override object ListToObject(List<int> result)
		{
			return new Vector2Int(result[0], result[1]);
		}
	}

	public class Vector3IntMonolineConverter : VectorIntConverter<Vector3Int>
	{
		protected override List<int> ObjectToList(object value)
		{
			var vector = (Vector3Int)value;
			return new List<int>() { vector.x, vector.y, vector.z };
		}

		protected override object ListToObject(List<int> result)
		{
			return new Vector3Int(result[0], result[1], result[2]);
		}
	}
}
