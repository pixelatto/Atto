using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atto.Extensions
{
	public static class ArrayExtensions
	{

		public static T PickRandom<T>(this T[] array)
		{
			return array[Random.Range(0, array.Length)];
		}

	}
}