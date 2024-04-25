using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class Camera2CameraSync : MonoBehaviour
{
    public Camera target;

    public bool syncPosition = false;
    public bool syncOrthoSize = false;

    Camera self;

    void LateUpdate()
    {
        if (self == null)
        {
            self = GetComponent<Camera>();
        }

        if (target != null)
        {
            if (syncPosition)
            {
                transform.position = target.transform.position;
            }
            if (syncOrthoSize)
            {
                self.orthographicSize = target.orthographicSize;
            }
        }
    }
}
