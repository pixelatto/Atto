using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CellularAutomata : MonoBehaviour
{
    public CellularMaterials materials;
    public float updateRate = 0.1f;
    public UpdateType updateType = UpdateType.Manual; public enum UpdateType { Manual, Automatic }
    public CellMaterial mouseSpawnMaterial = CellMaterial.Dirt;

    public static LayerMask layerMask => LayerMask.GetMask("Terrain");

    public CellularChunk currentChunk;

    float lastUpdateTime = 0;
    SpriteRenderer spriteRenderer;
    Texture2D texture;

    public static CellularAutomata instance { get { if (instance_ == null) { instance_ = FindObjectOfType<CellularAutomata>(); } return instance_; } }
    static CellularAutomata instance_;

    public bool hasChanged = false;

    private void Update()
    {
        if (currentChunk != null)
        {
            CheckTexture();

            switch (updateType)
            {
                case UpdateType.Manual:
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        Step();
                        RenderChunk();
                    }
                    break;
                case UpdateType.Automatic:
                    if (Time.time - lastUpdateTime > updateRate)
                    {
                        lastUpdateTime = Time.time;
                        Step();
                        RenderChunk();
                    }
                    break;
            }

            if (Debug.isDebugBuild)
            {
                DebugControls();
            }
        }
    }

    private void DebugControls()
    {
        int brushSize = 3;
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pixelPosition = currentChunk.WorldToPixelPosition(worldPosition);
        if (Input.GetMouseButton(0))
        {
            for (int i = -brushSize; i < brushSize; i++)
            {
                for (int j = -brushSize; j < brushSize; j++)
                {
                    currentChunk.SetValue(pixelPosition + new Vector2Int(i, j), mouseSpawnMaterial);
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            for (int i = -brushSize; i < brushSize; i++)
            {
                for (int j = -brushSize; j < brushSize; j++)
                {
                    currentChunk.SetValue(pixelPosition + new Vector2Int(i, j), CellMaterial.None);
                }
            }
        }
    }

    public CellMaterial[,] CurrentCellData()
    {
        return currentChunk.cells;
    }

    public void LoadData(Vector2 worldPosition, Vector2Int pixelSize, CellMaterial[,] cells)
    {
        currentChunk = new CellularChunk(worldPosition, pixelSize, cells);
    }

    bool[,] usedPositions;

    bool flip = false;

    void Step()
    {
        hasChanged = false;

        usedPositions = new bool[currentChunk.pixelSize.x, currentChunk.pixelSize.y];
        usedPositions.Fill(false);

        flip = !flip;

        for (int j = 0; j < currentChunk.pixelSize.y; j++)
        {
            for (int i = 0; i < currentChunk.pixelSize.x; i++)
            {
                int x = flip ? (currentChunk.pixelSize.x - 1 - i) : i;
                int y = j;
                var currentPosition = new Vector2Int(x, y);
                var currentMaterial = currentChunk.GetValue(currentPosition);

                bool usedPosition = usedPositions[x, y];

                if (!usedPosition && currentMaterial != CellMaterial.None)
                {
                    var movementType = materials.FindMaterial(currentMaterial).movement;

                    if (movementType == CellMovement.Static)
                    {
                        SwapCells(currentPosition, currentPosition);
                    }
                    else if (movementType == CellMovement.Granular)
                    {
                        var bottomPosition = new Vector2Int(x, y - 1);
                        var bottomLeftPosition = new Vector2Int(x - 1, y - 1);
                        var bottomRightPosition = new Vector2Int(x + 1, y - 1);
                        var canFallDown = currentChunk.CanDisplace(currentPosition, bottomPosition);
                        if (canFallDown)
                        {
                            SwapCells(currentPosition, bottomPosition);
                        }
                        else
                        {
                            var canFallLeft = currentChunk.CanDisplace(currentPosition, bottomLeftPosition);
                            var canFallRight = currentChunk.CanDisplace(currentPosition, bottomRightPosition);
                            if (canFallLeft || canFallRight)
                            {
                                int direction = (canFallLeft ? -1 : 0) + (canFallRight ? 1 : 0);
                                if (direction == 0)
                                {
                                    direction = (Random.value > 0.5f) ? 1 : -1;
                                }
                                SwapCells(currentPosition, currentPosition + new Vector2Int(direction, -1));
                            }
                        }
                    }
                    else if (movementType == CellMovement.Fluid)
                    {
                        var bottomPosition = new Vector2Int(x, y - 1);
                        var bottomLeftPosition = new Vector2Int(x - 1, y - 1);
                        var bottomRightPosition = new Vector2Int(x + 1, y - 1);
                        var canFallDown = currentChunk.CanDisplace(currentPosition, bottomPosition);
                        if (canFallDown)
                        {
                            SwapCells(currentPosition, bottomPosition);
                        }
                        else
                        {
                            var leftPosition = new Vector2Int(x - 1, y);
                            var rightPosition = new Vector2Int(x + 1, y);
                            var canFallLeft = currentChunk.CanDisplace(currentPosition, bottomLeftPosition);
                            var canFallRight = currentChunk.CanDisplace(currentPosition, bottomRightPosition);
                            var canSlideLeft = currentChunk.CanDisplace(currentPosition, leftPosition);
                            var canSlideRight = currentChunk.CanDisplace(currentPosition, rightPosition);
                            if (canFallLeft || canFallRight)
                            {
                                int direction = (canFallLeft ? -1 : 0) + (canFallRight ? 1 : 0);
                                if (direction == 0)
                                {
                                    direction = (Random.value > 0.5f) ? 1 : -1;
                                }
                                SwapCells(currentPosition, currentPosition + new Vector2Int(direction, -1));
                            }
                            else if (canSlideLeft || canSlideRight)
                            {
                                int direction = (canSlideLeft ? -1 : 0) + (canSlideRight ? 1 : 0);
                                if (direction == 0)
                                {
                                    if (Random.value > 0.95)
                                    {
                                        DestroyCell(currentPosition);
                                    }
                                    else
                                    {
                                        direction = (Random.value > 0.5f) ? 1 : -1;
                                    }
                                }
                                if (direction != 0)
                                {
                                    int distance = 0;
                                    int fall = 0;
                                    var fluidity = materials.FindMaterial(currentMaterial).fluidity;
                                    for (int spread = 1; spread < fluidity; spread++)
                                    {
                                        var spreadPosition = currentPosition + new Vector2Int(spread * direction, 0);
                                        var spreadPositionBottom = currentPosition + new Vector2Int(spread * direction, -1);
                                        if (currentChunk.CanDisplace(currentPosition, spreadPositionBottom))
                                        {
                                            distance = spread;
                                            fall = -1;
                                            break;
                                        }
                                        else if (!currentChunk.CanDisplace(currentPosition, spreadPosition))
                                        {
                                            distance = spread - 1;
                                            break;
                                        }
                                    }
                                    distance = Mathf.Max(0, distance);
                                    SwapCells(currentPosition, currentPosition + new Vector2Int(direction * distance, fall));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void DestroyCell(Vector2Int position)
    {
        currentChunk.SetValue(position, CellMaterial.None);
    }

    void SwapCells(Vector2Int oldPosition, Vector2Int newPosition)
    {
        if (oldPosition == newPosition) { return; }
        if (newPosition.x >= 0 && newPosition.y >= 0 && newPosition.x <= currentChunk.pixelSize.x - 1 && newPosition.y <= currentChunk.pixelSize.y - 1)
        {
            if (usedPositions[newPosition.x, newPosition.y])
            {
                return;
            }
            else
            {
                var material = currentChunk.GetValue(oldPosition);
                var otherMaterial = currentChunk.GetValue(newPosition);
                currentChunk.SetValue(newPosition, material);
                currentChunk.SetValue(oldPosition, otherMaterial);
                usedPositions[newPosition.x, newPosition.y] = true;
                hasChanged = true;
            }
        }
    }

    private void CheckTexture()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = this.GetOrAddComponent<SpriteRenderer>();
        }
        if (spriteRenderer.sprite == null || spriteRenderer.sprite.texture.width != currentChunk.pixelSize.x || spriteRenderer.sprite.texture.height != currentChunk.pixelSize.y)
        {
            texture = new Texture2D(currentChunk.pixelSize.x, currentChunk.pixelSize.y);
            texture.filterMode = FilterMode.Point;
            for (int i = 0; i < currentChunk.pixelSize.x; i++)
            {
                for (int j = 0; j < currentChunk.pixelSize.y; j++)
                {
                    texture.SetPixel(i, j, Color.clear);
                }
            }
            texture.Apply();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, currentChunk.pixelSize.x, currentChunk.pixelSize.y), Vector2.one * 0.5f, 8f);
        }
    }

    private void RenderChunk()
    {
        texture.SetPixels32(ChunkToColorArray());
        texture.Apply();
    }

    public Color32[] ChunkToColorArray()
    {
        Color32[] result = new Color32[currentChunk.pixelSize.x * currentChunk.pixelSize.y];

        for (int i = 0; i < currentChunk.pixelSize.x; i++)
        {
            for (int j = 0; j < currentChunk.pixelSize.y; j++)
            {
                int index = j * currentChunk.pixelSize.x + i;
                result[index] = Cell2Color(currentChunk.cells[i, j]);
            }
        }

        return result;
    }

    public Color32 Cell2Color(CellMaterial cellType)
    {
        if (cellType == CellMaterial.None)
        {
            return Color.clear;
        }
        else
        {
            return materials.FindMaterial(cellType).color;
        }
    }

    public void PixelsToSolids(Texture2D terrainRaster)
    {
        for (int i = 0; i < terrainRaster.width; i++)
        {
            for (int j = 0; j < terrainRaster.height; j++)
            {
                var position = new Vector2Int(i, j);
                var color = terrainRaster.GetPixel(i, j);
                if (color.r == 0 && color.g == 0 && color.b == 0)
                {
                    currentChunk.SetValue(position, CellMaterial.None);
                }
                else
                {
                    currentChunk.SetValue(position, CellMaterial.Rock);
                }
            }
        }
    }

}

public enum CellMaterial { None = 0, Rock = 1, Dirt = 2, Water = 3 }
public enum CellMovement { Undefined, Static, Granular, Fluid }
