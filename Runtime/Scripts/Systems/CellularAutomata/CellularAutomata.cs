using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    public CellularMaterials materials;
    public float updateRate = 0.1f;
    public UpdateType updateType = UpdateType.Manual; public enum UpdateType { Manual, Automatic }

    public PixelCamera pixelCamera;

    public static LayerMask layerMask => LayerMask.GetMask("Terrain");

    float lastUpdateTime = 0;

    public static CellularAutomata instance { get { if (instance_ == null) { instance_ = FindObjectOfType<CellularAutomata>(); } return instance_; } }
    static CellularAutomata instance_;

    public bool hasChanged = false;

    bool flip = false;

    public static Vector2Int roomPixelSize = new Vector2Int(128, 72);
    public static Vector2 roomWorldSize = new Vector2(128 / 8f, 72 / 8f);

    public static Cell emptyCell = new Cell(CellMaterial.None);

    public static uint currentTick = 0;

    private void Start()
    {
        currentTick = 0;
        var chunks = FindObjectsOfType<CellularChunk>();
        foreach (var chunk in chunks)
        {
            chunk.InitChunk();
        }
    }

    private void Update()
    {
        currentTick++;

        switch (updateType)
        {
            case UpdateType.Manual:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Step();
                }
                break;
            case UpdateType.Automatic:
                if (Time.time - lastUpdateTime > updateRate)
                {
                    lastUpdateTime = Time.time;
                    Step();
                }
                break;
        }
    }

    void Step()
    {
        hasChanged = false;

        for (int j = pixelCamera.lookAheadPixelRect.y; j <= pixelCamera.lookAheadPixelRect.y + pixelCamera.lookAheadPixelRect.height; j++)
        {
            for (int i = pixelCamera.lookAheadPixelRect.x; i <= pixelCamera.lookAheadPixelRect.x + pixelCamera.lookAheadPixelRect.width; i++)
            {
                int x = flip ? (pixelCamera.lookAheadPixelRect.x + pixelCamera.lookAheadPixelRect.width - (i - pixelCamera.lookAheadPixelRect.x)) : i;

                int y = j;
                var currentPosition = new Vector2Int(x, y);
                var currentCell = GetCell(currentPosition);

                if (currentCell.material != CellMaterial.None && !currentCell.wasUpdatedThisTick)
                {
                    var movementType = currentCell.movement;

                    if (movementType == CellMovement.Static)
                    {
                        SwapCells(currentPosition, currentPosition);
                    }
                    else if (movementType == CellMovement.Granular)
                    {
                        var bottomPosition = new Vector2Int(x, y - 1);
                        var bottomLeftPosition = new Vector2Int(x - 1, y - 1);
                        var bottomRightPosition = new Vector2Int(x + 1, y - 1);
                        var canFallDown = CanDisplace(currentPosition, bottomPosition);
                        if (canFallDown)
                        {
                            SwapCells(currentPosition, bottomPosition);
                        }
                        else
                        {
                            var canFallLeft = CanDisplace(currentPosition, bottomLeftPosition);
                            var canFallRight = CanDisplace(currentPosition, bottomRightPosition);
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
                        var canFallDown = CanDisplace(currentPosition, bottomPosition);
                        if (canFallDown)
                        {
                            SwapCells(currentPosition, bottomPosition);
                        }
                        else
                        {
                            var leftPosition = new Vector2Int(x - 1, y);
                            var rightPosition = new Vector2Int(x + 1, y);
                            var canFallLeft = CanDisplace(currentPosition, bottomLeftPosition);
                            var canFallRight = CanDisplace(currentPosition, bottomRightPosition);
                            var canSlideLeft = CanDisplace(currentPosition, leftPosition);
                            var canSlideRight = CanDisplace(currentPosition, rightPosition);
                            /*
                            if (canFallLeft || canFallRight)
                            {
                                int direction = (canFallLeft ? -1 : 0) + (canFallRight ? 1 : 0);
                                if (direction == 0)
                                {
                                    direction = (Random.value > 0.5f) ? 1 : -1;
                                }
                                SwapCells(currentPosition, currentPosition + new Vector2Int(direction, -1));
                            }
                            else */
                            if (canSlideLeft || canSlideRight)
                            {
                                int direction = (canSlideLeft ? -1 : 0) + (canSlideRight ? 1 : 0);
                                if (direction == 0)
                                {
                                    direction = (Random.value > 0.5f) ? 1 : -1;
                                }
                                int distance = 0;
                                int fall = 0;
                                var fluidity = currentCell.fluidity;
                                for (int spread = 1; spread < fluidity; spread++)
                                {
                                    var spreadPosition = currentPosition + new Vector2Int(spread * direction, 0);
                                    var spreadPositionBottom = currentPosition + new Vector2Int(spread * direction, -1);
                                    if (CanDisplace(currentPosition, spreadPositionBottom))
                                    {
                                        distance = spread;
                                        fall = -1;
                                        break;
                                    }
                                    else if (!CanDisplace(currentPosition, spreadPosition))
                                    {
                                        distance = spread - 1;
                                        break;
                                    }
                                }
                                distance = Mathf.Max(1, distance);
                                SwapCells(currentPosition, currentPosition + new Vector2Int(direction * distance, fall));
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tries to destroy a cell and returns true if suceeded
    /// </summary>
    public bool DestroyCell(Vector2Int globalPixelPosition)
    {
        var cell = SetCell(globalPixelPosition, new Cell(CellMaterial.None));
        return cell == null;
    }

    public Cell CreateCell(Vector2Int globalPixelPosition, CellMaterial material)
    {
        var newCell = new Cell(material);
        SetCell(globalPixelPosition, newCell);
        return newCell;
    }

    void SwapCells(Vector2Int oldPosition, Vector2Int newPosition)
    {
        if (oldPosition == newPosition) { return; }
        var cell = GetCell(oldPosition);
        var otherCell = GetCell(newPosition);
        SetCell(newPosition, cell);
        SetCell(oldPosition, otherCell);
        hasChanged = true;
    }

    public static Vector2Int GetPixelChunkAddress(int x, int y)
    {
        return new Vector2Int(Mathf.FloorToInt((float)x / roomPixelSize.x), Mathf.FloorToInt((float)y / roomPixelSize.y));
    }

    public static CellularChunk FindChunk(Vector2Int chunkAddress)
    {
        if (CellularChunk.chunkDirectory.ContainsKey(chunkAddress.x) && CellularChunk.chunkDirectory[chunkAddress.x].ContainsKey(chunkAddress.y))
        {
            return CellularChunk.chunkDirectory[chunkAddress.x][chunkAddress.y];
        }
        else
        {
            return null;
        }
    }

    Cell SetCell(Vector2Int globalPixelPosition, Cell cell)
    {
        return SetCell(globalPixelPosition.x, globalPixelPosition.y, cell);
    }

    Cell SetCell(int x, int y, Cell cell)
    {
        Vector2Int chunkAddress = GetPixelChunkAddress(x, y);
        CellularChunk targetChunk = FindChunk(chunkAddress);

        if (targetChunk == null)
        {
            //Debug.Log("Can't find chunk at " + chunkAddress);
            return null;
        }

        Vector2Int localChunkCoords = new Vector2Int(x - targetChunk.pixelPosition.x, y - targetChunk.pixelPosition.y);

        cell.lastUpdateTick = currentTick;
        targetChunk[localChunkCoords.x, localChunkCoords.y] = cell;
        return targetChunk[localChunkCoords.x, localChunkCoords.y];
    }

    public Cell GetCell(Vector2Int globalPixelPosition)
    {
        return GetCell(globalPixelPosition.x, globalPixelPosition.y);
    }

    Cell GetCell(int x, int y)
    {
        Vector2Int chunkAddress = GetPixelChunkAddress(x, y);
        CellularChunk targetChunk = FindChunk(chunkAddress);

        if (targetChunk == null)
        {
            return emptyCell;
        }
        else
        {
            Vector2Int localChunkCoords = new Vector2Int(x - targetChunk.pixelPosition.x, y - targetChunk.pixelPosition.y);
            return targetChunk[localChunkCoords.x, localChunkCoords.y];
        }
    }

    public static Vector2Int WorldToPixelPosition(Vector3 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x * 8f), Mathf.RoundToInt(position.y * 8f));
    }

    public static Vector3 PixelToWorldPosition(Vector2Int pixelPosition)
    {
        return new Vector3((pixelPosition.x + 0.5f) / 8f, (pixelPosition.y + 0.5f) / 8f, 0);
    }

    public bool CanDisplace(Vector2Int origin, Vector2Int target)
    {
        var originCell = GetCell(origin);
        var targetCell = GetCell(target);

        if (targetCell == originCell)
        {
            return false;
        }
        else if (targetCell.material == CellMaterial.None)
        {
            return true;
        }
        else
        {
            if (targetCell.movement == CellMovement.Static)
            {
                return false;
            }
            else if (originCell.movement == CellMovement.Granular && targetCell.movement == CellMovement.Fluid)
            {
                return true;
            }
        }
        return false;
    }
}

public enum CellMaterial { None = 0, Rock = 1, Dirt = 2, Water = 3 }
public enum CellMovement { Undefined, Static, Granular, Fluid }

