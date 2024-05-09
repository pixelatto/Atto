using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MeshFilterSync : MonoBehaviour
{
    public MeshFilter target;

    MeshFilter self;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (self == null)
        {
            self = GetComponent<MeshFilter>();
        }

        if (target != null)
        {
            self.sharedMesh = target.sharedMesh;
        }
    }
}