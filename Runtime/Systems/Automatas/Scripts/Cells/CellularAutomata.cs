using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellularAutomata : SingletonMonobehaviour<CellularAutomata>
{
    public float updateRate = 0.1f;
    public UpdateType updateType = UpdateType.Automatic;
    public enum UpdateType { Manual, Automatic }

    public PixelCamera pixelCamera;

    public static LayerMask layerMask => LayerMask.GetMask("Terrain");

    public bool hasChanged = false;

    public static Cell emptyCell = new Cell(CellMaterial.Empty);
    public static Cell solidCell = new Cell(CellMaterial.Stone);

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
            List<int> xIndices = new List<int>();
            for (int i = pixelCamera.lookAheadPixelRect.x; i <= pixelCamera.lookAheadPixelRect.x + pixelCamera.lookAheadPixelRect.width; i++)
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

                        switch (movementType)
                        {
                            case CellMovement.Static:
                                break;
                            case CellMovement.Structural:
                                StructuralMovement();
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

    private void StructuralMovement()
    {
        if (currentCell.needsUpdate)
        {
            UpdateStructuralConnections(currentPosition);
        }

        if (currentCell.isStructurallyConnected)
        {
            return;
        }
        else
        {
            GranularMovement();
        }
    }

    private void UpdateStructuralConnections(Vector2Int position)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> toVisit = new Queue<Vector2Int>();
        bool isConnectedToStatic = false;

        toVisit.Enqueue(position);

        while (toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();
            if (!visited.Contains(current))
            {
                visited.Add(current);
                var currentCell = GetCell(current);

                if (currentCell.material == CellMaterial.Empty)
                {
                    continue; // No consideres células vacías en la lógica de conexión
                }

                if (currentCell.movement == CellMovement.Static)
                {
                    isConnectedToStatic = true;
                }

                if (currentCell.movement == CellMovement.Structural)
                {
                    Vector2Int[] neighbors = {
                        new Vector2Int(current.x + 1, current.y),
                        new Vector2Int(current.x - 1, current.y),
                        new Vector2Int(current.x, current.y + 1),
                        new Vector2Int(current.x, current.y - 1)
                    };

                    foreach (var neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            toVisit.Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        // Actualizar todas las células visitadas basándose en el estado de conexión encontrado
        foreach (var cellPosition in visited)
        {
            var cell = GetCell(cellPosition);
            if (cell.movement == CellMovement.Structural)
            {
                cell.isStructurallyConnected = isConnectedToStatic;
                cell.needsUpdate = false; // Marcar como no necesita actualización
            }
        }
    }

    private void FluidMovement()
    {
        int x = currentPosition.x;
        int y = currentPosition.y;
        var bottomPosition = new Vector2Int(x, y - 1);
        var bottomCell = GetCell(bottomPosition);

        if (CanDisplace(currentCell, bottomCell))
        {
            SwapCells(currentPosition, bottomPosition);
        }
        else
        {
            var fluidity = currentCell.fluidity;
            if (fluidity < 1) { Debug.LogWarning("Fluidity must be at least 1 for fluids to work properly."); }

            Cell leftSuccess = null;
            Vector2Int leftSuccessPosition = currentPosition;
            for (int i = 1; i < fluidity; i++)
            {
                var leftPosition = currentPosition + new Vector2Int(-i, 0);
                var leftCell = GetCell(leftPosition, false);
                var leftBelowPosition = currentPosition + new Vector2Int(-i, -1);
                var leftBelowCell = GetCell(leftBelowPosition, false);

                if (leftCell.IsSolid())
                {
                    break; // Stop if we encounter a solid cell
                }

                if (CanDisplace(currentCell, leftBelowCell))
                {
                    leftSuccess = leftBelowCell;
                    leftSuccessPosition = leftBelowPosition;
                    break;
                }
                else if (!CanDisplace(currentCell, leftCell))
                {
                    break;
                }
            }
            if (leftSuccess == null)
            {
                for (int i = 1; i < fluidity; i++)
                {
                    var leftPosition = currentPosition + new Vector2Int(-i, 0);
                    var leftCell = GetCell(leftPosition, false);
                    if (leftCell.IsSolid())
                    {
                        break; // Stop if we encounter a solid cell
                    }

                    if (CanDisplace(currentCell, leftCell))
                    {
                        leftSuccess = leftCell;
                        leftSuccessPosition = leftPosition;
                        break;
                    }
                }
            }

            Cell rightSuccess = null;
            Vector2Int rightSuccessPosition = currentPosition;
            for (int i = 1; i < fluidity; i++)
            {
                var rightPosition = currentPosition + new Vector2Int(i, 0);
                var rightCell = GetCell(rightPosition, false);
                var rightBelowPosition = currentPosition + new Vector2Int(i, -1);
                var rightBelowCell = GetCell(rightBelowPosition, false);

                if (rightCell.IsSolid())
                {
                    break; // Stop if we encounter a solid cell
                }

                if (CanDisplace(currentCell, rightBelowCell))
                {
                    rightSuccess = rightBelowCell;
                    rightSuccessPosition = rightBelowPosition;
                    break;
                }
            }
            if (rightSuccess == null)
            {
                for (int i = 1; i < fluidity; i++)
                {
                    var rightPosition = currentPosition + new Vector2Int(i, 0);
                    var rightCell = GetCell(rightPosition, false);
                    if (rightCell.IsSolid())
                    {
                        break; // Stop if we encounter a solid cell
                    }

                    if (CanDisplace(currentCell, rightCell))
                    {
                        rightSuccess = rightCell;
                        rightSuccessPosition = rightPosition;
                        break;
                    }
                }
            }

            if (rightSuccess != null || leftSuccess != null)
            {
                int direction = ((leftSuccess != null) ? -1 : 0) + ((rightSuccess != null) ? 1 : 0);
                if (direction == 0)
                {
                    direction = (Random.value > 0.5f) ? 1 : -1;
                }
                if (direction == -1)
                {
                    SwapCells(currentPosition, leftSuccessPosition);
                }
                else
                {
                    SwapCells(currentPosition, rightSuccessPosition);
                }
            }
        }
    }

    private void GranularMovement()
    {
        int x = currentPosition.x;
        int y = currentPosition.y;
        var bottomPosition = new Vector2Int(x, y - 1);
        var bottomCell = GetCell(bottomPosition, false);
        var canFallDown = CanDisplace(currentCell, bottomCell);

        if (canFallDown)
        {
            SwapCells(currentPosition, bottomPosition);
        }
        else
        {
            int fluidity = currentCell.fluidity;

            if (fluidity > 0)
            {
                HandlePositiveFluidity(fluidity, x, y);
            }
            else if (fluidity == 0)
            {
                // No lateral movement, only stack vertically
                return;
            }
            else
            {
                HandleNegativeFluidity(-fluidity, x, y);
            }
        }
    }

    private void HandlePositiveFluidity(int fluidity, int x, int y)
    {
        var leftPositions = new List<Vector2Int>();
        var rightPositions = new List<Vector2Int>();

        for (int i = 1; i <= fluidity; i++)
        {
            leftPositions.Add(new Vector2Int(x - i, y - 1));
            rightPositions.Add(new Vector2Int(x + i, y - 1));
        }

        var canFallLeft = leftPositions.Exists(pos => CanDisplace(currentCell, GetCell(pos, false)));
        var canFallRight = rightPositions.Exists(pos => CanDisplace(currentCell, GetCell(pos, false)));

        if (canFallLeft || canFallRight)
        {
            int direction = (canFallLeft ? -1 : 0) + (canFallRight ? 1 : 0);
            if (direction == 0)
            {
                direction = (Random.value > 0.5f) ? 1 : -1;
            }
            var targetPosition = direction == -1 ? leftPositions.First(pos => CanDisplace(currentCell, GetCell(pos, false)))
                                                 : rightPositions.First(pos => CanDisplace(currentCell, GetCell(pos, false)));
            SwapCells(currentPosition, targetPosition);
        }
    }

    private void HandleNegativeFluidity(int absFluidity, int x, int y)
    {
        var leftBottomPosition = new Vector2Int(x - 1, y - absFluidity);
        var rightBottomPosition = new Vector2Int(x + 1, y - absFluidity);

        var leftBottomCell = GetCell(leftBottomPosition, false);
        var leftBelowLeftBottomCell = GetCell(new Vector2Int(x - 1, y - absFluidity - 1), false);

        var rightBottomCell = GetCell(rightBottomPosition, false);
        var rightBelowRightBottomCell = GetCell(new Vector2Int(x + 1, y - absFluidity - 1), false);

        bool canMoveLeft = leftBottomCell.material == CellMaterial.Empty && leftBelowLeftBottomCell.material != CellMaterial.Empty;
        bool canMoveRight = rightBottomCell.material == CellMaterial.Empty && rightBelowRightBottomCell.material != CellMaterial.Empty;

        if (canMoveLeft && canMoveRight)
        {
            var targetPosition = (Random.value > 0.5f) ? leftBottomPosition : rightBottomPosition;
            SwapCells(currentPosition, targetPosition);
        }
        else if (canMoveLeft)
        {
            SwapCells(currentPosition, leftBottomPosition);
        }
        else if (canMoveRight)
        {
            SwapCells(currentPosition, rightBottomPosition);
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
                var targetPosition = new Vector2Int(x + i, y + j - currentCell.gravity);
                var targetCell = GetCell(targetPosition);

                if (CanDisplace(currentCell, targetCell))
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

        if (cell == null || otherCell == null)
        {
            Debug.LogWarning("Attempting to swap null cells.");
            return;
        }

        SetCell(newPosition, cell);
        SetCell(oldPosition, otherCell);
        hasChanged = true;

        // Actualizar conexiones estructurales
        UpdateStructuralConnections(oldPosition);
        UpdateStructuralConnections(newPosition);

        // Propagar el estado de conexión a las células adyacentes
        PropagateStructuralUpdate(oldPosition);
        PropagateStructuralUpdate(newPosition);
    }

    public void DestroyCell(Vector2Int globalPixelPosition)
    {
        SetCell(globalPixelPosition, new Cell(CellMaterial.Empty));
        UpdateStructuralConnections(globalPixelPosition);

        // Propagar el estado de conexión a las células adyacentes
        PropagateStructuralUpdate(globalPixelPosition);
    }

    public Cell CreateCell(Vector2Int globalPixelPosition, CellMaterial material)
    {
        var newCell = new Cell(material);
        SetCell(globalPixelPosition, newCell);
        UpdateStructuralConnections(globalPixelPosition);

        // Propagar el estado de conexión a las células adyacentes
        PropagateStructuralUpdate(globalPixelPosition);

        return newCell;
    }

    private void PropagateStructuralUpdate(Vector2Int position)
    {
        Vector2Int[] neighbors = {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1)
        };

        foreach (var neighbor in neighbors)
        {
            var neighborCell = GetCell(neighbor);
            if (neighborCell != null && neighborCell.movement == CellMovement.Structural)
            {
                neighborCell.needsUpdate = true; // Marcar las células adyacentes como que necesitan actualización
            }
        }
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
            //Debug.LogWarning($"Chunk not found at {chunkAddress}");
            return null;
        }

        var localChunkCoords = GlobalPixelToChunkCoords(globalPixelPosition, targetChunk);
        if (localChunkCoords == null)
        {
            //Debug.LogWarning($"Invalid local chunk coordinates for position {globalPixelPosition}");
            return null;
        }
        else
        {
            cell.lastUpdateTick = currentTick;
            // Inicializar la temperatura de la célula al establecerla en el mundo
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
            return fallBackToEmpty ? emptyCell : solidCell;
        }
        else
        {
            Vector2Int localChunkCoords = new Vector2Int(globalPixelPosition.x - targetChunk.pixelPosition.x, globalPixelPosition.y - targetChunk.pixelPosition.y);
            var cell = targetChunk[localChunkCoords.x, localChunkCoords.y];
            return cell ?? emptyCell;
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
        else
        {
            return (targetCell.movement > originCell.movement);
        }
    }
}
