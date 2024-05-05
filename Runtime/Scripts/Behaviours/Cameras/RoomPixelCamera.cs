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
        /*
        if (currentRoom != null)
        {
            OnRoomEnter(null, currentRoom);
        }
        */
    }

    /*
    public void OnRoomEnter(CameraTarget target, Room room)
    {
        if (target != null)
        {
            target.GetComponent<Rigidbody2D>().simulated = false;
        }
        var previousRoom = currentRoom;
        if (previousRoom != null)
        {
            previousRoom.SetMainLayer(true);
        }

        currentRoom = room;
        currentRoom.SetMainLayer(true);
        currentRoom.SetDecorationsLayer(true);
        currentRoom.SetLightsLayer(true);
        CellularAutomata.instance.SetColliders(false);

        zoom = room.cameraZoom;
        Vector2 error = (Vector3)currentRoom.roomRect.center - transform.position;
        transform.position += (Vector3)error;
        cam.orthographicSize = baseCameraSize * zoom;

        CellularAutomata.instance.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, CellularAutomata.instance.transform.position.z);
        CellularAutomata.instance.SetColliders(true);

        currentRoom.SetMainLayer(false);
        currentRoom.SetDecorationsLayer(false);
        currentRoom.SetLightsLayer(false);

        if (target != null)
        {
            target.GetComponent<Rigidbody2D>().simulated = true;
        }
    }
    */
}
