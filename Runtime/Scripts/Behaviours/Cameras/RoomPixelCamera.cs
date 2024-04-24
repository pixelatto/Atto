using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPixelCamera : PixelCamera
{
    Camera cam;
    Room targetRoom = null;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void OnRoomEnter(Room room)
    {
        targetRoom = room;
        zoom = room.cameraZoom;
    }

    void Update()
    {
        if (targetRoom != null)
        {
            Vector2 error = (Vector3)targetRoom.rect.center - transform.position;
            transform.position += (Vector3)error;
            cam.orthographicSize = baseCameraSize * zoom;
        }
    }
}
