using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RoomPixelCamera : PixelCamera
{
    public Ease transition;
    public float transitionTime = 0.35f;
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

        cam.orthographicSize = baseCameraSize * zoom; //TODO: Tween

        var targetPosition = new Vector3(currentRoom.roomRect.center.x, currentRoom.roomRect.center.y, cam.transform.position.z);
        transform.DOMove(targetPosition, transitionTime).SetEase(transition);
    }
}
