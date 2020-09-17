using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    
    public static T PickRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

}

public static class ListExtensions
{

    public static T PickRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

}

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