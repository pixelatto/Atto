using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateContinously : MonoBehaviour
{
    public Vector3 axis = Vector3.forward;
    public float rotationSpeed = 100f;
	
	void Update ()
    {
        var angle = Time.deltaTime * rotationSpeed;
        transform.Rotate(axis, angle);
	}
}
