using UnityEngine;

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
}
