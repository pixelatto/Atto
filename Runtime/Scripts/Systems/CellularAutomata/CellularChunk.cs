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

    Grid tilemapGrid;
    Tilemap tilemap;
    TilemapCollider2D tilemapCollider;
    CompositeCollider2D compositeCollider;
    TilemapRenderer tilemapRenderer;
    Rigidbody2D rb2d;

    public CellularColliderType cellularColliderType = CellularColliderType.Main; public enum CellularColliderType { Main, Lights }

    TileBase emptyTile;
    TileBase fullTile;
    TileBase ulTile;
    TileBase urTile;
    TileBase blTile;
    TileBase brTile;

    private void Update()
    {
        if (textureDirty)
        {
            RenderChunk();
            textureDirty = false;

            RecalculateFullCollider();
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
        PixelsToCells(currentTerrainRaster, CellMaterial.Rock);
        PaintSurfacePixels(surfaceColor);
        RasterLightBlockers();
        CheckTexture();
        RenderChunk();
        GetComponent<Room>().ReplaceWithChunk(this);
        SetupColliders();

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

    public void PixelsToCells(Texture2D terrainRaster, CellMaterial cellMaterial)
    {
        for (int i = 0; i < terrainRaster.width; i++)
        {
            for (int j = 0; j < terrainRaster.height; j++)
            {
                var color = terrainRaster.GetPixel(i, j);
                var newCell = new Cell(CellMaterial.None);
                if (color.r == 0 && color.g == 0 && color.b == 0)
                {
                    newCell.material = CellMaterial.None;
                }
                else
                {
                    newCell.material = cellMaterial;
                    newCell.color = color;
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
                        cell.color = mainSurfaceColor;
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
                var position = worldPosition + new Vector2(i + 0.5f, j + 0.5f) / 8f;
                var hit = Physics2D.OverlapPoint(position, lightBlockersLayerMask);
                cell.blocksLight = hit != null;
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

    public void SetupColliders()
    {
        GameObject tilemapObject = new GameObject("Tilemap");
        tilemapObject.transform.parent = transform;
        tilemapObject.transform.localPosition = Vector3.zero;
        tilemapGrid = tilemapObject.GetOrAddComponent<Grid>();
        tilemap = tilemapObject.GetOrAddComponent<Tilemap>();
        tilemapCollider = tilemapObject.GetOrAddComponent<TilemapCollider2D>();
        compositeCollider = tilemapObject.GetOrAddComponent<CompositeCollider2D>();
        tilemapRenderer = tilemapObject.GetOrAddComponent<TilemapRenderer>();
        rb2d = tilemapObject.GetOrAddComponent<Rigidbody2D>();

        tilemapObject.layer = LayerMask.NameToLayer("Terrain");
        tilemapRenderer.enabled = false;
        tilemapGrid.cellSize = new Vector3(1f / 8f, 1f / 8f, 1);
        tilemapCollider.usedByComposite = true;
        rb2d.bodyType = RigidbodyType2D.Static;
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;

        RecalculateFullCollider();
    }

    public void RecalculateFullCollider()
    {
        if (emptyTile == null) { emptyTile = Resources.Load<TileBase>("Tiles/Tile_Empty"); }
        if (fullTile == null)  { fullTile  = Resources.Load<TileBase>("Tiles/Tile_Full"); }
        if (ulTile == null)    { ulTile    = Resources.Load<TileBase>("Tiles/Tile_UL"); }
        if (urTile == null)    { urTile    = Resources.Load<TileBase>("Tiles/Tile_UR"); }
        if (blTile == null)    { blTile    = Resources.Load<TileBase>("Tiles/Tile_BL"); }
        if (brTile == null)    { brTile    = Resources.Load<TileBase>("Tiles/Tile_BR"); }

        bool current = false, top = false, bottom = false, left = false, right = false;

        for (int i = 0; i < CellularAutomata.roomPixelSize.x; i++)
        {
            for (int j = 0; j < CellularAutomata.roomPixelSize.y; j++)
            {
                var currentPosition = new Vector2Int(i, j);
                var currentTilePosition = new Vector3Int(i, j, 0);
                var currentCell = this[currentPosition.x, currentPosition.y];
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
                    var topCell = this[currentPosition.x, currentPosition.y + 1];
                    var bottomCell = this[currentPosition.x, currentPosition.y - 1];
                    var leftCell = this[currentPosition.x - 1, currentPosition.y];
                    var rightCell = this[currentPosition.x + 1, currentPosition.y];

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
                            tilemap.SetTile(currentTilePosition, fullTile);
                            break;
                        case 3:
                            tilemap.SetTile(currentTilePosition, fullTile);
                            break;
                        case 2:
                            if (top && right) { tilemap.SetTile(currentTilePosition, blTile); }
                            else if (right && bottom) { tilemap.SetTile(currentTilePosition, ulTile); }
                            else if (bottom && left) { tilemap.SetTile(currentTilePosition, urTile); }
                            else if (left && top) { tilemap.SetTile(currentTilePosition, brTile); }
                            break;
                        case 1:
                            tilemap.SetTile(currentTilePosition, emptyTile);
                            break;
                        case 0:
                            tilemap.SetTile(currentTilePosition, emptyTile);
                            break;
                    }
                }
                else
                {
                    tilemap.SetTile(currentTilePosition, emptyTile);
                }
            }
        }
        compositeCollider.GenerateGeometry();
    }

}
