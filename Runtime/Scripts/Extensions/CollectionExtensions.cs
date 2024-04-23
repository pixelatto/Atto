using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    
    public static T PickRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public static List<T> ToList<T>(this T[] array)
    {
        var result = new List<T>();
        for (int i = 0; i < array.Length; i++)
        {
            result.Add(array[i]);
        }
        return result;
    }

    public static int Sum(this IList<int> list)
    {
        int result = 0;
        for (int i = 0; i < list.Count; i++)
        {
            result += list[i];
        }
        return result;
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