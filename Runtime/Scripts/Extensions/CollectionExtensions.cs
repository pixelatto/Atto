using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
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

    public static T PickRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public static void Fill<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
    }

    public static void Fill<T>(this T[,] array, T value)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] = value;
            }
        }
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
}

public static class ListExtensions
{

    public static T PickRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static void AddIfNotListed<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
        }
    }

    public static void RemoveIfListed<T>(this List<T> list, T item)
    {
        if (list.Contains(item))
        {
            list.Remove(item);
        }
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