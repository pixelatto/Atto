using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellularAutomata : SingletonMonobehaviour<CellularAutomata>
{
    public static LayerMask layerMask => LayerMask.GetMask("Terrain");

    public static Cell solidCell = new Cell(CellMaterial.Stone);

    public static uint currentTick = 0;

    Vector2Int currentPosition;
    Cell currentCell;

    private StructuralConnectionManager structuralConnectionManager;
    private Dictionary<CellMovement, MovementHandler> movementHandlers;

    bool isDirty = false;

    Cycle updateRateCycle;

    private void Start()
    {
        currentTick = 0;
        structuralConnectionManager = new StructuralConnectionManager(this);

        movementHandlers = new Dictionary<CellMovement, MovementHandler>
        {
            { CellMovement.Static, new StaticMovementHandler(this) },
            { CellMovement.Structural, new StructuralMovementHandler(this) },
            { CellMovement.Granular, new GranularMovementHandler(this) },
            { CellMovement.Liquid, new LiquidMovementHandler(this) },
            { CellMovement.Gas, new GasMovementHandler(this) }
        };

        var chunks = FindObjectsOfType<CellularChunk>();
        foreach (var chunk in chunks)
        {
            chunk.InitChunk();
        }

        updateRateCycle = new Cycle(1f/ Global.simulationsFPS, SetDirty);
        updateRateCycle.Start();
    }

    void SetDirty()
    {
        isDirty = true;
    }

    private void Update()
    {
        if (isDirty)
        {
            Step();
            isDirty = false;
        }
    }

    void Step()
    {
        currentTick++;

        for (int j = PixelCamera.instance.lookAheadPixelRect.y; j <= PixelCamera.instance.lookAheadPixelRect.y + PixelCamera.instance.lookAheadPixelRect.height; j++)
        {
            List<int> xIndices = new List<int>();
            for (int i = PixelCamera.instance.lookAheadPixelRect.x; i <= PixelCamera.instance.lookAheadPixelRect.x + PixelCamera.instance.lookAheadPixelRect.width; i++)
            {
                xIndices.Add(i);
            }
            xIndices.Shuffle();

            foreach (int x in xIndices)
            {
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
                    if (currentCell.material != CellMaterial.Empty && !currentCell.wasUpdatedThisTick)
                    {
                        var movementType = currentCell.movement;
                        movementHandlers[movementType].SetCurrentState(currentPosition, currentCell);
                        movementHandlers[movementType].Move();
                    }
                }
            }
        }
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

    public void SwapCells(Vector2Int oldPosition, Vector2Int newPosition)
    {
        if (oldPosition == newPosition) { return; }
        var cell = GetCell(oldPosition);
        var otherCell = GetCell(newPosition);

        if (cell == null || otherCell == null)
        {
            Debug.LogWarning("Attempting to swap null cells.");
            return;
        }

        SetCell(newPosition, cell);
        SetCell(oldPosition, otherCell);

        UpdateStructuralConnections(oldPosition);
        UpdateStructuralConnections(newPosition);

        PropagateStructuralUpdate(oldPosition);
        PropagateStructuralUpdate(newPosition);
    }

    public void DestroyCell(Vector2Int globalPixelPosition)
    {
        SetCell(globalPixelPosition, new Cell(CellMaterial.Empty));
        UpdateStructuralConnections(globalPixelPosition);

        PropagateStructuralUpdate(globalPixelPosition);
    }

    public Cell CreateCell(Vector2Int globalPixelPosition, CellMaterial material)
    {
        var newCell = new Cell(material);
        SetCell(globalPixelPosition, newCell);
        UpdateStructuralConnections(globalPixelPosition);

        PropagateStructuralUpdate(globalPixelPosition);

        return newCell;
    }

    public void DisplaceFluid(Vector2Int fluidPosition)
    {
        var fluidCell = GetCell(fluidPosition);
        var directions = new Vector2Int[]
        {
        new Vector2Int(1, 0),  // Right
        new Vector2Int(-1, 0), // Left
        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 1),  // Up-Right
        new Vector2Int(-1, 1)  // Up-Left
        };

        foreach (var direction in directions)
        {
            var newPosition = fluidPosition + direction;
            var newCell = GetCell(newPosition);
            if (newCell.IsEmpty())
            {
                SwapCells(fluidPosition, newPosition);
                return;
            }
        }

        // Si no hay posiciones vacías alrededor, tratar de mover lateralmente hacia abajo
        foreach (var direction in directions)
        {
            var newPosition = fluidPosition + new Vector2Int(direction.x, direction.y - 1);
            var newCell = GetCell(newPosition);
            if (newCell.IsEmpty())
            {
                SwapCells(fluidPosition, newPosition);
                return;
            }
        }
    }

    public void UpdateStructuralConnections(Vector2Int position)
    {
        structuralConnectionManager.UpdateStructuralConnections(position);
    }

    public void PropagateStructuralUpdate(Vector2Int position)
    {
        structuralConnectionManager.PropagateStructuralUpdate(position);
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
            if (cell.elapsedLifetime == 0)
            {
                cell.temperature = cell.startTemperature;
            }
            targetChunk[((Vector2Int)localChunkCoords).x, ((Vector2Int)localChunkCoords).y] = cell;
            return targetChunk[((Vector2Int)localChunkCoords).x, ((Vector2Int)localChunkCoords).y];
        }
    }

    public Cell GetCell(Vector2Int globalPixelPosition, bool fallBackToEmpty = true)
    {
        Vector2Int chunkAddress = GetPixelChunkAddress(globalPixelPosition);
        CellularChunk targetChunk = FindChunk(chunkAddress);

        if (targetChunk == null)
        {
            return fallBackToEmpty ? new Cell(CellMaterial.Empty) : solidCell;
        }
        else
        {
            Vector2Int localChunkCoords = new Vector2Int(globalPixelPosition.x - targetChunk.pixelPosition.x, globalPixelPosition.y - targetChunk.pixelPosition.y);
            var cell = targetChunk[localChunkCoords.x, localChunkCoords.y];
            return cell ?? new Cell(CellMaterial.Empty);
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
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x * Global.pixelsPerUnit), Mathf.FloorToInt(worldPosition.y * Global.pixelsPerUnit));
    }

    public static Vector3 PixelToWorldPosition(Vector2Int globalPixelPosition)
    {
        return new Vector3((globalPixelPosition.x + 0.5f) / Global.pixelsPerUnit, (globalPixelPosition.y + 0.5f) / Global.pixelsPerUnit, 0);
    }

    public bool CanDisplace(Cell originCell, Cell targetCell)
    {
        if (targetCell == originCell)
        {
            return false;
        }
        else if (targetCell.material == CellMaterial.Empty)
        {
            return true;
        }
        else if ((targetCell.IsLiquid() || targetCell.IsGas()) && originCell.IsSolid())
        {
            return true;
        }
        else
        {
            return (targetCell.movement > originCell.movement);
        }
    }
}
