using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPixelCamera : PixelCamera
{
    public Room currentRoom = null;
    [HideInInspector]public Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (currentRoom != null)
        {
            OnRoomEnter(currentRoom);
        }
    }

    public void OnRoomEnter(Room room)
    {
        currentRoom = room;

        zoom = room.cameraZoom;
        Vector2 error = (Vector3)currentRoom.roomRect.center - transform.position;
        transform.position += (Vector3)error;
        cam.orthographicSize = baseCameraSize * zoom;
    }
}
