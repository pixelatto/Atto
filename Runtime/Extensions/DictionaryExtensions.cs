using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atto.Extensions
{
	public static class DictionaryExtensions
	{

		public static V GetOrNull<K, V>(this Dictionary<K, V> dictionary, K key)
		{
			if (dictionary.ContainsKey(key))
			{
				return dictionary[key];
			}
			else
			{
				return default(V);
			}
		}

	}
}