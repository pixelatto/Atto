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

        public static int CountNotNullElements<T>(this T[] array)
        {
            int count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    count++;
                }
            }
            return count;
        }

        public static T FirstNotNullElement<T>(this T[] array)
        {
            if (array == null)
            {
                return default(T);
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    return array[i];
                }
            }
            return default(T);
        }
    }

}