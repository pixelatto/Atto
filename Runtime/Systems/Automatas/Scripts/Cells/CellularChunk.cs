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

    public Vector2 worldPosition => new Vector2(pixelPosition.x / Global.pixelsPerUnit, pixelPosition.y / Global.pixelsPerUnit);
    public Vector2 worldSize => new Vector2(pixelSize.x / Global.pixelsPerUnit, pixelSize.y / Global.pixelsPerUnit);

    public static Dictionary<int, Dictionary<int, CellularChunk>> chunkDirectory = new Dictionary<int, Dictionary<int, CellularChunk>>();

    bool chunkDirty = true;

    CellularChunkCollider[] chunkColliders;
    CellularChunkRenderer[] chunkRenderers;

    public Cell this[int x, int y]
    {
        get
        {
            var index = Index(x, y);
            if (index < 0 || index > cells.Length - 1)
            {
                return new Cell(CellMaterial.Empty); // Crear una nueva célula vacía en lugar de usar una compartida
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
            chunkDirty = true;
            cells[index] = value;
        }
    }

    private int Index(int x, int y)
    {
        return y * pixelSize.x + x;
    }

    private void Update()
    {
        if (chunkDirty)
        {
            chunkDirty = false;

            foreach (var chunkCollider in chunkColliders)
            {
                chunkCollider.Recalculate();
            }

            foreach (var chunkRenderer in chunkRenderers)
            {
                chunkRenderer.Draw();
            }
        }
    }

    public void InitChunk()
    {
        ClearCells();
        BuildColliders();
        BuildRenderers();
        RasterChunk();
        AdressChunk();
    }

    private void ClearCells()
    {
        cells = new Cell[pixelSize.x * pixelSize.y];
        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                cells[Index(i, j)] = new Cell(CellMaterial.Empty);
            }
        }
    }

    private void AdressChunk()
    {
        chunkAddress = new Vector2Int(Mathf.RoundToInt(pixelPosition.x / Global.roomPixelSize.x), Mathf.RoundToInt(pixelPosition.y / Global.roomPixelSize.y));
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

    private void RasterChunk()
    {
        var currentTerrainRaster = CellularRasterizer.instance.RasterChunk(this);
        RasterTerrain(currentTerrainRaster);
        RasterSurfacePixels(surfaceColor);
        RasterLightBlockers();
        GetComponent<Room>().Hide();
    }

    private void BuildColliders()
    {
        gameObject.AddComponent<CellularChunkCollider>();
        gameObject.AddComponent<CellularChunkCollider>();

        chunkColliders = GetComponents<CellularChunkCollider>();

        chunkColliders[0].InitChunkCollider(this, CellularChunkCollider.CellularColliderType.Main);
        chunkColliders[1].InitChunkCollider(this, CellularChunkCollider.CellularColliderType.Lights);
    }

    private void BuildRenderers()
    {
        gameObject.AddComponent<CellularChunkRenderer>();
        gameObject.AddComponent<CellularChunkRenderer>();
        gameObject.AddComponent<CellularChunkRenderer>();

        chunkRenderers = GetComponents<CellularChunkRenderer>();

        chunkRenderers[0].InitChunkRenderer(this, CellRenderLayer.Back);
        chunkRenderers[1].InitChunkRenderer(this, CellRenderLayer.Main);
        chunkRenderers[2].InitChunkRenderer(this, CellRenderLayer.Front);
    }

    public void RasterTerrain(Texture2D terrainRaster)
    {
        for (int i = 0; i < terrainRaster.width; i++)
        {
            for (int j = 0; j < terrainRaster.height; j++)
            {
                Color32 color = terrainRaster.GetPixel(i, j);
                var newCell = new Cell(CellMaterial.Empty);
                if (color.r == 0 && color.g == 0 && color.b == 0)
                {
                    newCell.material = CellMaterial.Empty;
                }
                else
                {
                    foreach (var material in CellularMaterials.instance.materials)
                    {
                        if (color.r == material.identifierColor.r && color.g == material.identifierColor.g && color.b == material.identifierColor.b)
                        {
                            newCell.material = material.cellMaterial;
                        }
                    }
                }
                newCell.temperature = newCell.startTemperature;
                cells[Index(i, j)] = newCell;
            }
        }
    }

    private Vector2 PixelToWorldPosition(int i, int j)
    {
        return worldPosition + new Vector2((i + 0.5f) / Global.pixelsPerUnit, (j + 0.5f) / Global.pixelsPerUnit);
    }

    public void RasterSurfacePixels(Color mainSurfaceColor)
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
        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                var cell = cells[Index(i, j)];
                if (cell.IsFluid())
                {
                    cell.blocksLight = false;
                }
                else
                {
                    int distanceToEmpty = GetDistanceToEmpty(i, j);
                    cell.blocksLight = distanceToEmpty >= 2;
                }
            }
        }
    }

    public int GetDistanceToEmpty(int x, int y)
    {
        int width = pixelSize.x;
        int height = pixelSize.y;

        Queue<(int, int, int)> queue = new Queue<(int, int, int)>();
        bool[,] visited = new bool[width, height];

        queue.Enqueue((x, y, 0));
        visited[x, y] = true;

        int[][] directions = new int[][]
        {
            new int[] { -1, 0 },
            new int[] { 1, 0 },
            new int[] { 0, -1 },
            new int[] { 0, 1 }
        };

        while (queue.Count > 0)
        {
            var (currentX, currentY, distance) = queue.Dequeue();

            if (!this[currentX, currentY].IsSolid())
            {
                return distance;
            }

            foreach (var dir in directions)
            {
                int newX = currentX + dir[0];
                int newY = currentY + dir[1];

                if (newX >= 0 && newX < width && newY >= 0 && newY < height && !visited[newX, newY])
                {
                    queue.Enqueue((newX, newY, distance + 1));
                    visited[newX, newY] = true;
                }
            }
        }

        return int.MaxValue;
    }

    public Color32[] ChunkToColorArray(CellRenderLayer renderLayer)
    {
        Color32[] result = new Color32[pixelSize.x * pixelSize.y];

        for (int i = 0; i < pixelSize.x; i++)
        {
            for (int j = 0; j < pixelSize.y; j++)
            {
                int index = j * pixelSize.x + i;
                var currentCell = cells[Index(i, j)];
                if (currentCell.renderLayer == renderLayer)
                {
                    result[index] = currentCell.GetColor();
                }
                else
                {
                    result[index] = Color.clear;
                }
            }
        }

        return result;
    }

}
