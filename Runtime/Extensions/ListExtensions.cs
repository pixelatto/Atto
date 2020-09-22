using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atto.Extensions
{
	public static class ListExtensions
	{

		public static T PickRandom<T>(this List<T> list)
		{
			return list[Random.Range(0, list.Count)];
		}

	}
}