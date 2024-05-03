using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularCollider : MonoBehaviour
{
    CellularAutomata automata;
    Tilemap tilemap;
    CompositeCollider2D compositeCollider;

    public TileBase emptyTile;
    public TileBase solidTile;
    public TileBase ulTile;
    public TileBase urTile;
    public TileBase blTile;
    public TileBase brTile;


    public float updateRate = 0.1f;

    float lastUpdateTime = 0;

    bool isDirty = true;

    private void Awake()
    {
        automata = GetComponentInChildren<CellularAutomata>();
        tilemap = GetComponentInChildren<Tilemap>();
        compositeCollider = GetComponentInChildren<CompositeCollider2D>();
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
                compositeCollider.GenerateGeometry();
                isDirty = false;
                lastUpdateTime = Time.time;
            }

            tilemap.transform.localPosition = -((Vector2)(automata.currentChunk.pixelSize) / 8f) / 2f;
        }
    }

    private void RecalculateFullCollider()
    {
        for (int i = 0; i < automata.currentChunk.pixelSize.x; i++)
        {
            for (int j = 0; j < automata.currentChunk.pixelSize.y; j++)
            {
                var currentPosition = new Vector2Int(i, j);
                var isSolid = automata.currentChunk.IsSolid(currentPosition);
                if (isSolid)
                {
                    var topTile    = automata.currentChunk.IsSolid(currentPosition + Vector2Int.up);
                    var bottomTile = automata.currentChunk.IsSolid(currentPosition + Vector2Int.down);
                    var leftTile   = automata.currentChunk.IsSolid(currentPosition + Vector2Int.left);
                    var rightTile  = automata.currentChunk.IsSolid(currentPosition + Vector2Int.right);
                    int solidCount = (topTile ? 1 : 0) + (bottomTile ? 1 : 0) + (leftTile ? 1 : 0) + (rightTile ? 1 : 0);
                    switch (solidCount)
                    {
                        case 4:
                            tilemap.SetTile(new Vector3Int(i, j, 0), solidTile);
                            break;
                        case 3:
                            tilemap.SetTile(new Vector3Int(i, j, 0), solidTile);
                            break;
                        case 2:
                            if (topTile && rightTile) { tilemap.SetTile(new Vector3Int(i, j, 0), blTile); }
                            else if (rightTile && bottomTile) { tilemap.SetTile(new Vector3Int(i, j, 0), ulTile); }
                            else if (bottomTile && leftTile) { tilemap.SetTile(new Vector3Int(i, j, 0), urTile); }
                            else if (leftTile && topTile) { tilemap.SetTile(new Vector3Int(i, j, 0), brTile); }
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
    }

}