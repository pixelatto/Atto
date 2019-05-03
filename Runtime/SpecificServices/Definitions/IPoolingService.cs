using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolingService
{

    GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation);
    void Despawn(GameObject target);

}
