using UnityEngine;
using System.Linq;

public static class TransformExtensions
{
    public static void SortChildrenAlphabetically(this Transform parent)
    {
        var children = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }

        System.Array.Sort(children, (x, y) => string.Compare(x.name, y.name));

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }

    public static void ShuffleChildren(this Transform transform)
    {
        var children = transform.Cast<Transform>().ToList();

        foreach (Transform child in children)
        {
            child.SetParent(null);
        }

        var shuffledChildren = children.OrderBy(x => Random.value).ToList();

        foreach (Transform child in shuffledChildren)
        {
            child.SetParent(transform);
        }
    }
}
