using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePoolingService : IPoolingService
{

    Dictionary<GameObject, List<GameObject>> prefabToObjectsRegistry = new Dictionary<GameObject, List<GameObject>>();

    public SimplePoolingService()
    {

    }

    public void Despawn(GameObject target)
    {
        target.SetActive(false);
    }

    public GameObject Spawn(GameObject sourcePrefab, Vector3 position, Quaternion rotation)
    {
        GameObject result = null;
        if (!prefabToObjectsRegistry.ContainsKey(sourcePrefab))
        {
            result = CreateNewInstance(sourcePrefab, position, rotation);
        }
        else
        {
            List<GameObject> targetList = prefabToObjectsRegistry[sourcePrefab];
            foreach (var target in targetList)
            {
                if (!target.activeSelf)
                {
                    target.transform.position = position;
                    target.transform.rotation = rotation;
                    target.SetActive(true);
                    result = target;
                    break;
                }
            }
            if (result == null)
            {
                CreateNewInstance(sourcePrefab, position, rotation);
            }
        }
        return result;
    }

    private GameObject CreateNewInstance(GameObject sourcePrefab, Vector3 position, Quaternion rotation)
    {
        GameObject newObject = GameObject.Instantiate(sourcePrefab, position, rotation);
        if (!prefabToObjectsRegistry.ContainsKey(sourcePrefab))
        {
            prefabToObjectsRegistry.Add(sourcePrefab, new List<GameObject>());
        }
        prefabToObjectsRegistry[sourcePrefab].Add(newObject);
        return newObject;
    }
}
