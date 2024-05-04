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
        if (automata.currentChunk != null)
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

            tilemap.transform.localPosition = -((Vector2)(automata.currentChunk.pixelSize) / 8f) / 2f;
        }
    }

    public void RecalculateFullCollider()
    {
        bool current = false, top = false, bottom = false, left = false, right = false;

        for (int i = 0; i < automata.currentChunk.pixelSize.x; i++)
        {
            for (int j = 0; j < automata.currentChunk.pixelSize.y; j++)
            {
                var currentPosition = new Vector2Int(i, j);
                var currentCell = automata.currentChunk.GetCell(currentPosition);
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
                    var topCell = automata.currentChunk.GetCell(currentPosition + Vector2Int.up);
                    var bottomCell = automata.currentChunk.GetCell(currentPosition + Vector2Int.down);
                    var leftCell = automata.currentChunk.GetCell(currentPosition + Vector2Int.left);
                    var rightCell = automata.currentChunk.GetCell(currentPosition + Vector2Int.right);

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
                            tilemap.SetTile(new Vector3Int(i, j, 0), solidTile);
                            break;
                        case 3:
                            tilemap.SetTile(new Vector3Int(i, j, 0), solidTile);
                            break;
                        case 2:
                            if (top && right) { tilemap.SetTile(new Vector3Int(i, j, 0), blTile); }
                            else if (right && bottom) { tilemap.SetTile(new Vector3Int(i, j, 0), ulTile); }
                            else if (bottom && left) { tilemap.SetTile(new Vector3Int(i, j, 0), urTile); }
                            else if (left && top) { tilemap.SetTile(new Vector3Int(i, j, 0), brTile); }
                            break;
                        case 1:
                            tilemap.SetTile(new Vector3Int(i, j, 0), emptyTile);
                            break;
                        case 0:
                            tilemap.SetTile(new Vector3Int(i, j, 0), emptyTile);
                            break;
                    }
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), emptyTile);
                }
            }
        }
        compositeCollider.GenerateGeometry();
    }

}