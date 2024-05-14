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
        var bottomCell = GetCell(bottomPosition);
        var bottomReaction = CellularMaterials.instance.FindReaction(currentCell.material, bottomCell.material);
        var canReactDown = bottomReaction != null;
        var canFallDown = CanDisplace(currentCell, bottomCell);

        if (canReactDown)
        {
            ReactCells(currentPosition, bottomPosition, bottomReaction);
        }
        else if (canFallDown)
        {
            SwapCells(currentPosition, bottomPosition);
        }
        else
        {
            var leftPosition = new Vector2Int(x - 1, y);
            var rightPosition = new Vector2Int(x + 1, y);
            var leftCell = GetCell(leftPosition);
            var rightCell = GetCell(rightPosition);
            var leftReaction = CellularMaterials.instance.FindReaction(currentCell.material, leftCell.material);
            var rightReaction = CellularMaterials.instance.FindReaction(currentCell.material, rightCell.material);
            var canReactLeft = leftReaction != null;
            var canReactRight = rightReaction != null;
            var canSlideLeft = CanDisplace(currentCell, leftCell);
            var canSlideRight = CanDisplace(currentCell, rightCell);

            if (canReactLeft || canReactRight)
            {
                int direction = (canReactLeft ? -1 : 0) + (canReactRight ? 1 : 0);
                if (direction == 0)
                {
                    direction = (Random.value > 0.5f) ? 1 : -1;
                }
                ReactCells(currentPosition, currentPosition + new Vector2Int(direction, 0), direction == -1 ? leftReaction : rightReaction);
            }
            else if (canSlideLeft || canSlideRight)
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
                    var spreadPositionCell = GetCell(spreadPosition);
                    var spreadPositionBottomCell = GetCell(spreadPositionBottom);
                    var spreadPositionReaction = CellularMaterials.instance.FindReaction(currentCell.material, spreadPositionCell.material);
                    var spreadPositionBottomReaction = CellularMaterials.instance.FindReaction(currentCell.material, spreadPositionBottomCell.material);
                    var canReactSpreadPosition = spreadPositionReaction != null;
                    var canReactSpreadPositionBottom = spreadPositionBottomReaction != null;

                    if (canReactSpreadPositionBottom)
                    {
                        distance = spread;
                        fall = -1;
                        ReactCells(currentPosition, spreadPositionBottom, spreadPositionBottomReaction);
                        break;
                    }
                    else if (canReactSpreadPosition)
                    {
                        distance = spread;
                        ReactCells(currentPosition, spreadPosition, spreadPositionReaction);
                        break;
                    }
                    else if (CanDisplace(currentCell, spreadPositionBottomCell))
                    {
                        distance = spread;
                        fall = -1;
                        break;
                    }
                    else if (!CanDisplace(currentCell, spreadPositionCell))
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
        var bottomCell = GetCell(bottomPosition);
        var bottomLeftPosition = new Vector2Int(x - 1, y - 1);
        var bottomRightPosition = new Vector2Int(x + 1, y - 1);
        var canFallDown = CanDisplace(currentCell, bottomCell);
        var downReaction = CellularMaterials.instance.FindReaction(currentCell.material, bottomCell.material);
        var canReactDown = downReaction != null;
        if (canReactDown)
        {
            ReactCells(currentPosition, bottomPosition, downReaction);
        }
        if (canFallDown)
        {
            SwapCells(currentPosition, bottomPosition);
        }
        else
        {
            var bottomLeftCell = GetCell(bottomLeftPosition);
            var bottomRightCell = GetCell(bottomRightPosition);
            var canFallLeft = CanDisplace(currentCell, bottomLeftCell);
            var canFallRight = CanDisplace(currentCell, bottomRightCell);
            var bottomLeftReaction = CellularMaterials.instance.FindReaction(currentCell.material, bottomLeftCell.material);
            var canReactBottomLeft = bottomLeftReaction != null;
            var bottomRightReaction = CellularMaterials.instance.FindReaction(currentCell.material, bottomRightCell.material);
            var canReactBottomRight = bottomRightReaction != null;

            if (canReactBottomLeft || canReactBottomRight)
            {
                int direction = (canFallLeft ? -1 : 0) + (canFallRight ? 1 : 0);
                if (direction == 0)
                {
                    direction = (Random.value > 0.5f) ? 1 : -1;
                }
                ReactCells(currentPosition, currentPosition + new Vector2Int(direction, -1), direction == -1 ? bottomLeftReaction : bottomRightReaction);
            }
            else if (canFallLeft || canFallRight)
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
                var targetPosition = new Vector2Int(x+i, y+j - currentCell.gravity);
                var targetCell = GetCell(targetPosition);

                var reaction = CellularMaterials.instance.FindReaction(currentCell.material, targetCell.material);
                var canReact = reaction != null;
                if (canReact)
                {
                    targetPositions.Add(targetPosition);
                }
                else if (CanDisplace(currentCell, targetCell))
                {
                    targetPositions.Add(targetPosition);
                }
            }
        }
        if (targetPositions.Count > 0)
        {
            var randomPosition = targetPositions.PickRandom();
            var randomCell = GetCell(randomPosition);
            var reaction = CellularMaterials.instance.FindReaction(currentCell.material, randomCell.material);
            var canReact = reaction != null;
            if (canReact)
            {
                ReactCells(currentPosition, randomPosition, reaction);
            }
            else
            {
                SwapCells(currentPosition, randomPosition);
            }
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

    void ReactCells(Vector2Int reactorPositionA, Vector2Int reactorPositionB, CellMaterialReaction reaction)
    {
        Debug.Log("Reaction! " + reaction.reactionName);
        var cellA = GetCell(reactorPositionA);
        var cellB = GetCell(reactorPositionB);
        bool reversed = reaction.reactorA == cellB.material && reaction.reactorB == cellA.material;

        var productA = reversed ? reaction.productB : reaction.productA;
        var productB = reversed ? reaction.productA : reaction.productB;
        SetCell(reactorPositionA, new Cell(productA));
        SetCell(reactorPositionB, new Cell(productB));
        hasChanged = true;
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

    public bool CanDisplace(Cell originCell, Cell targetCell)
    {
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
            return (targetCell.movement > originCell.movement);
        }
    }
}

