using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject FindOrAddChild(this GameObject gameObject, string childName)
    {
        var existingChild = gameObject.transform.Find(childName);
        if (existingChild != null)
        {
            return existingChild.gameObject;
        }
        else
        {
            var newChild = new GameObject(childName);
            newChild.transform.SetParent(gameObject.transform);
            newChild.transform.localPosition = Vector3.zero;
            return newChild;
        }
    }
}