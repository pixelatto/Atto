using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : SingletonMonobehaviour<CellularAutomata>
{
    public float updateRate = 0.1f;
    public UpdateType updateType = UpdateType.Automatic; public enum UpdateType { Manual, Automatic }

    public PixelCamera pixelCamera;

    public static LayerMask layerMask => LayerMask.GetMask("Terrain");

    public bool hasChanged = false;

    bool flip = false;

    public static Cell emptyCell = new Cell(CellMaterial.None);

    public static uint currentTick = 0;

    float lastUpdateTime = 0;

    Vector2Int currentPosition;
    Cell currentCell;

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
                currentPosition = new Vector2Int(x, y);
                currentCell = GetCell(currentPosition);
                currentCell.elapsedLifetime++;
                if (currentCell != null && currentCell.lifetimeTimeout)
                {
                    currentCell.Destroy();
                }
                else
                {
                    if (currentCell.material != CellMaterial.None && !currentCell.wasUpdatedThisTick)
                    {
                        var movementType = currentCell.movement;

                        switch (movementType)
                        {
                            case CellMovement.Static:
                                break;
                            case CellMovement.Granular:
                                GranularMovement();
                                break;
                            case CellMovement.Fluid:
                                FluidMovement();
                                break;
                            case CellMovement.Gas:
                                GasMovement();
                                break;
                        }
                    }
                }
            }
        }
    }

    private void FluidMovement()
    {
        int x = currentPosition.x;
        int y = currentPosition.y;
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
            //var canFallLeft = CanDisplace(currentPosition, bottomLeftPosition);
            //var canFallRight = CanDisplace(currentPosition, bottomRightPosition);
            var canSlideLeft = CanDisplace(currentPosition, leftPosition);
            var canSlideRight = CanDisplace(currentPosition, rightPosition);
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

    private void GranularMovement()
    {
        int x = currentPosition.x;
        int y = currentPosition.y;
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

    private void GasMovement()
    {
        int x = currentPosition.x;
        int y = currentPosition.y;

        List<Vector2Int> targetPositions = new List<Vector2Int>();
        var fluidity = currentCell.fluidity;
        for (int i = -fluidity; i <= fluidity; i++)
        {
            for (int j = -fluidity; j <= fluidity; j++)
            {
                var targetPosition = new Vector2Int(x+i, y+j + currentCell.gravity);
                var targetCell = GetCell(targetPosition);
                if (targetCell.IsEmpty() || targetCell.IsGas())
                {
                    targetPositions.Add(targetPosition);
                }
            }
        }
        if (targetPositions.Count > 0)
        {
            var randomPosition = targetPositions.PickRandom();
            SwapCells(currentPosition, randomPosition);
        }
    }

    public void DestroyCell(Vector2Int globalPixelPosition)
    {
        SetCell(globalPixelPosition, new Cell(CellMaterial.None));
    }

    public Cell CreateCell(Vector2Int globalPixelPosition, CellMaterial material)
    {
        var newCell = new Cell(material);
        SetCell(globalPixelPosition, newCell);
        return newCell;
    }

    public Cell CreateCellIfEmpty(Vector2Int globalPixelPosition, CellMaterial material)
    {
        var currentCell = GetCell(globalPixelPosition);
        if (currentCell.IsEmpty())
        {
            var newCell = new Cell(material);
            SetCell(globalPixelPosition, newCell);
            return newCell;
        }
        else
        {
            return null;
        }
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
        return GetPixelChunkAddress(new Vector2Int(x, y));
    }

    public static Vector2Int GetPixelChunkAddress(Vector2Int globalPixelPosition)
    {
        return new Vector2Int(Mathf.FloorToInt((float)globalPixelPosition.x / Global.roomPixelSize.x), Mathf.FloorToInt((float)globalPixelPosition.y / Global.roomPixelSize.y));
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
        Vector2Int chunkAddress = GetPixelChunkAddress(globalPixelPosition.x, globalPixelPosition.y);
        CellularChunk targetChunk = FindChunk(chunkAddress);

        if (targetChunk == null)
        {
            //Debug.Log("Can't find chunk at " + chunkAddress);
            return null;
        }

        var localChunkCoords = GlobalPixelToChunkCoords(globalPixelPosition, targetChunk);
        if (localChunkCoords == null)
        {
            return null;
        }
        else
        {
            cell.lastUpdateTick = currentTick;
            targetChunk[((Vector2Int)localChunkCoords).x, ((Vector2Int)localChunkCoords).y] = cell;
            return targetChunk[((Vector2Int)localChunkCoords).x, ((Vector2Int)localChunkCoords).y];
        }
    }

    public Cell GetCell(Vector2Int globalPixelPosition)
    {
        Vector2Int chunkAddress = GetPixelChunkAddress(globalPixelPosition);
        CellularChunk targetChunk = FindChunk(chunkAddress);

        if (targetChunk == null)
        {
            return emptyCell;
        }
        else
        {
            Vector2Int localChunkCoords = new Vector2Int(globalPixelPosition.x - targetChunk.pixelPosition.x, globalPixelPosition.y - targetChunk.pixelPosition.y);
            return targetChunk[localChunkCoords.x, localChunkCoords.y];
        }
    }

    public static Vector2Int? GlobalPixelToChunkCoords(Vector2Int globalPixelCoords, CellularChunk chunk)
    {
        var chunkCoords = new Vector2Int(globalPixelCoords.x - chunk.pixelPosition.x, globalPixelCoords.y - chunk.pixelPosition.y);
        if (chunkCoords.x < 0 || chunkCoords.y < 0 || chunkCoords.x >= chunk.pixelSize.x || chunkCoords.y >= chunk.pixelSize.y)
        {
            return null;
        }
        else
        {
            return chunkCoords;
        }
    }

    public static Vector2Int WorldToPixelPosition(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x * Global.pixelsPerUnit), Mathf.RoundToInt(worldPosition.y * Global.pixelsPerUnit));
    }

    public static Vector3 PixelToWorldPosition(Vector2Int globalPixelPosition)
    {
        return new Vector3((globalPixelPosition.x + 0.5f) / Global.pixelsPerUnit, (globalPixelPosition.y + 0.5f) / Global.pixelsPerUnit, 0);
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

