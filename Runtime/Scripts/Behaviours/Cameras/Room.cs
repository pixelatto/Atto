﻿using System;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Rect roomRect;
    public float cameraZoom = 1f;

    public Vector2 worldPosition => new Vector2(roomRect.x, roomRect.y);
    public Vector2 worldSize => new Vector2(roomRect.width, roomRect.height);
    public Vector2Int pixelSize => new Vector2Int((int)(roomRect.width * 8), (int)(roomRect.height * 8));

    RoomPixelCamera cam;

    public Transform mainLayer;
    public Transform backgroundLayer;
    public Transform decorationsLayer;
    public Transform lightsLayer;

    private void Awake()
    {
        cam = FindObjectOfType<RoomPixelCamera>();
    }

    public void ReplaceWithChunk(CellularChunk chunk)
    {
        SetMainLayer(false);
        SetDecorationsLayer(false);
        SetLightsLayer(false);
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        var target = collider.gameObject.GetComponent<CameraTarget>();
        if (target != null)
        {
            if (cam.currentRoom != this && roomRect.Shrink(0.25f).Contains(collider.transform.position))
            {
                //cam.OnRoomEnter(target, this);
            }
        }
    }

    public void SetMainLayer(bool active)
    {
        if (mainLayer != null)
            mainLayer.gameObject.SetActive(active);
    }

    public void SetBackgroundLayer(bool active)
    {
        if (backgroundLayer != null)
            backgroundLayer.gameObject.SetActive(active);
    }

    public void SetDecorationsLayer(bool active)
    {
        if (decorationsLayer != null)
            decorationsLayer.gameObject.SetActive(active);
    }

    public void SetLightsLayer(bool active)
    {
        if (lightsLayer != null)
            lightsLayer.gameObject.SetActive(active);
    }
}