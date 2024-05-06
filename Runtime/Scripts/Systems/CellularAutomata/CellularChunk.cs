using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularChunk : MonoBehaviour
{
    public Cell[] cells;

    public Vector2Int chunkAddress;
    public Vector2Int pixelPosition;
    public Vector2Int pixelSize;
    public Color surfaceColor;

    public Vector2 worldPosition => new Vector2(pixelPosition.x / 8f, pixelPosition.y / 8f);
    public Vector2 worldSize => new Vector2(pixelSize.x / 8f, pixelSize.y / 8f);

    SpriteRenderer spriteRenderer;
    Texture2D texture;

    public static Dictionary<int, Dictionary<int, CellularChunk>> chunkDirectory = new Dictionary<int, Dictionary<int, CellularChunk>>();

    public bool textureDirty = false;

    CellularChunkCollider[] chunkColliders;

    private void Update()
    {
        if (textureDirty)
        {
            RenderChunk();
            textureDirty = false;

            foreach (var chunkCollider in chunkColliders)
            {
                chunkCollider.Recalculate();
            }
        }
    }

    public void InitChunk()
    {
        cells = new Cell[pixelSize.x * pixelSize.y];
        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                cells[Index(i, j)] = new Cell(CellMaterial.None);
            }
        }

        var currentTerrainRaster = CellularRasterizer.instance.RasterChunk(this);
        PixelsToCells(currentTerrainRaster);
        PaintSurfacePixels(surfaceColor);
        RasterLightBlockers();
        CheckTexture();
        RenderChunk();
        GetComponent<Room>().ReplaceWithChunk(this);
        var mainChunkCollider = gameObject.AddComponent<CellularChunkCollider>();
        var lightsChunkCollider = gameObject.AddComponent<CellularChunkCollider>();
        lightsChunkCollider.cellularColliderType = CellularChunkCollider.CellularColliderType.Lights;
        chunkColliders = GetComponents<CellularChunkCollider>();
        foreach (var chunkCollider in chunkColliders)
        {
            chunkCollider.InitChunkCollider(this);
        }

        chunkAddress = new Vector2Int(Mathf.RoundToInt(pixelPosition.x / CellularAutomata.roomPixelSize.x), Mathf.RoundToInt(pixelPosition.y / CellularAutomata.roomPixelSize.y));
        if (!chunkDirectory.ContainsKey(chunkAddress.x))
        {
            chunkDirectory.Add(chunkAddress.x, new Dictionary<int, CellularChunk>());
        }
        if (!chunkDirectory[chunkAddress.x].ContainsKey(chunkAddress.y))
        {
            chunkDirectory[chunkAddress.x].Add(chunkAddress.y, this);
        }
        else
        {
            chunkDirectory[chunkAddress.x][chunkAddress.y] = this;
        }
    }

    public Cell this[int x, int y]
    {
        get
        {
            var index = Index(x, y);
            if (index < 0 || index > cells.Length - 1)
            {
                return CellularAutomata.emptyCell;
            };
            return cells[index];
        }
        set
        {
            var index = Index(x, y);
            if (index < 0 || index > cells.Length - 1)
            {
                return;
            };
            textureDirty = true;
            cells[index] = value;
        }
    }

    private int Index(int x, int y)
    {
        return y * pixelSize.x + x;
    }

    public void PixelsToCells(Texture2D terrainRaster)
    {
        for (int i = 0; i < terrainRaster.width; i++)
        {
            for (int j = 0; j < terrainRaster.height; j++)
            {
                Color32 color = terrainRaster.GetPixel(i, j);
                var newCell = new Cell(CellMaterial.None);
                if (color.r == 0 && color.g == 0 && color.b == 0)
                {
                    newCell.material = CellMaterial.None;
                }
                else
                {
                    bool found = false;
                    foreach (var material in CellularAutomata.instance.materials.materials)
                    {
                        if (color.r == material.identifierColor.r && color.g == material.identifierColor.g && color.b == material.identifierColor.b)
                        {
                            newCell.material = material.cellMaterial;
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        Debug.Log("Material color identifier not defined: " + color);
                    }
                }
                cells[Index(i, j)] = newCell;
            }
        }
    }

    public void PaintSurfacePixels(Color mainSurfaceColor)
    {
        if (mainSurfaceColor == Color.clear)
        {
            return;
        }

        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = pixelSize.y - 1; j >= 0; j--)
            {
                var cell = this[i, j];
                if (cell.IsSolid())
                {
                    if (j != pixelSize.y - 1)
                    {
                        cell.overrideColor = mainSurfaceColor;
                    }
                    break;
                }
            }
        }
    }

    public void RasterLightBlockers()
    {
        var lightBlockersLayerMask = LayerMask.GetMask("LightBlockers");
        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                var cell = cells[Index(i,j)];
                if (cell.IsLiquid())
                {
                    cell.blocksLight = false;
                }
                else
                {
                    var position = worldPosition + new Vector2(i + 0.5f, j + 0.5f) / 8f;
                    var hit = Physics2D.OverlapPoint(position, lightBlockersLayerMask);
                    cell.blocksLight = hit != null;
                }
            }
        }
    }

    private void RenderChunk()
    {
        texture.SetPixels32(ChunkToColorArray());
        texture.Apply();
    }

    private void CheckTexture()
    {
        if (spriteRenderer == null)
        {
            var childObject = new GameObject("ChunkRenderer");
            childObject.transform.SetParent(transform);
            childObject.transform.localPosition = new Vector3(pixelSize.x / 8f / 2f, pixelSize.y / 8f / 2f, 0);
            spriteRenderer = childObject.AddComponent<SpriteRenderer>();
        }
        if (spriteRenderer.sprite == null || spriteRenderer.sprite.texture.width != pixelSize.x || spriteRenderer.sprite.texture.height != pixelSize.y)
        {
            texture = new Texture2D(pixelSize.x, pixelSize.y);
            texture.filterMode = FilterMode.Point;
            for (int i = 0; i < pixelSize.x; i++)
            {
                for (int j = 0; j < pixelSize.y; j++)
                {
                    texture.SetPixel(i, j, Color.clear);
                }
            }
            texture.Apply();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, pixelSize.x, pixelSize.y), Vector2.one * 0.5f, 8f);
        }
    }

    public Color32[] ChunkToColorArray()
    {
        Color32[] result = new Color32[pixelSize.x * pixelSize.y];

        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                int index = j * pixelSize.x + i;
                result[index] = cells[Index(i, j)].GetColor();
            }
        }

        return result;
    }

}
