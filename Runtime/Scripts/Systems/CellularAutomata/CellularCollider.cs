using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularCollider : MonoBehaviour
{
    public CellularColliderType cellularColliderType = CellularColliderType.Main; public enum CellularColliderType { Main, Lights }

    public TileBase emptyTile;
    public TileBase solidTile;
    public TileBase ulTile;
    public TileBase urTile;
    public TileBase blTile;
    public TileBase brTile;

    CellularAutomata automata;
    Tilemap tilemap;

    [HideInInspector] public CompositeCollider2D compositeCollider;
    [HideInInspector] public TilemapCollider2D tilemapCollider;

    public float updateRate = 0.1f;

    float lastUpdateTime = 0;

    bool isDirty = true;

    private void Awake()
    {
        automata = GetComponentInParent<CellularAutomata>();
        tilemap = GetComponentInChildren<Tilemap>();
        compositeCollider = GetComponentInChildren<CompositeCollider2D>();
        tilemapCollider = GetComponentInChildren<TilemapCollider2D>();
    }

    private void Start()
    {

    }

    void Update()
    {
        if (Time.time - lastUpdateTime > updateRate)
        {
            isDirty = true;
        }

        if (isDirty)
        {
            RecalculateFullCollider();
            isDirty = false;
            lastUpdateTime = Time.time;
        }

        tilemap.transform.localPosition = -((Vector2)(CellularAutomata.roomPixelSize) / 8f) / 2f;
    }

    public void RecalculateFullCollider()
    {
        bool current = false, top = false, bottom = false, left = false, right = false;

        var pixelRect = CellularAutomata.instance.viewPortPixelRect;

        for (int i = pixelRect.x; i <= pixelRect.x + pixelRect.width; i++)
        {
            for (int j = pixelRect.y; j <= pixelRect.y + pixelRect.height; j++)
            {
                var currentLocalPosition = new Vector3Int(i - pixelRect.x, j - pixelRect.y, 0);

                var currentGlobalPosition = new Vector2Int(i, j);
                var currentCell = CellularAutomata.instance.GetCell(currentGlobalPosition);
                switch (cellularColliderType)
                {
                    case CellularColliderType.Main:
                        current = currentCell.IsSolid();
                        break;
                    case CellularColliderType.Lights:
                        current = currentCell.blocksLight;
                        break;
                }
                if (current)
                {
                    var topCell = CellularAutomata.instance.GetCell(currentGlobalPosition + Vector2Int.up);
                    var bottomCell = CellularAutomata.instance.GetCell(currentGlobalPosition + Vector2Int.down);
                    var leftCell = CellularAutomata.instance.GetCell(currentGlobalPosition + Vector2Int.left);
                    var rightCell = CellularAutomata.instance.GetCell(currentGlobalPosition + Vector2Int.right);

                    switch (cellularColliderType)
                    {
                        case CellularColliderType.Main:
                            top = topCell.IsSolid();
                            bottom = bottomCell.IsSolid();
                            left = leftCell.IsSolid();
                            right = rightCell.IsSolid();
                            break;
                        case CellularColliderType.Lights:
                            top = topCell.blocksLight;
                            bottom = bottomCell.blocksLight;
                            left = leftCell.blocksLight;
                            right = rightCell.blocksLight;
                            break;
                    }
                    int neighbourCount = (top ? 1 : 0) + (bottom ? 1 : 0) + (left ? 1 : 0) + (right ? 1 : 0);
                    switch (neighbourCount)
                    {
                        case 4:
                            tilemap.SetTile(currentLocalPosition, solidTile);
                            break;
                        case 3:
                            tilemap.SetTile(currentLocalPosition, solidTile);
                            break;
                        case 2:
                            if (top && right) { tilemap.SetTile(currentLocalPosition, blTile); }
                            else if (right && bottom) { tilemap.SetTile(currentLocalPosition, ulTile); }
                            else if (bottom && left) { tilemap.SetTile(currentLocalPosition, urTile); }
                            else if (left && top) { tilemap.SetTile(currentLocalPosition, brTile); }
                            break;
                        case 1:
                            tilemap.SetTile(currentLocalPosition, emptyTile);
                            break;
                        case 0:
                            tilemap.SetTile(currentLocalPosition, emptyTile);
                            break;
                    }
                }
                else
                {
                    tilemap.SetTile(currentLocalPosition, emptyTile);
                }
            }
        }
        compositeCollider.GenerateGeometry();
    }

}