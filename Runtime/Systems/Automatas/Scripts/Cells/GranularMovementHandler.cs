
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GranularMovementHandler : MovementHandler
{
    public GranularMovementHandler(CellularAutomata automata) : base(automata) { }

    public override void Move()
    {
        int x = currentPosition.x;
        int y = currentPosition.y;
        var bottomPosition = new Vector2Int(x, y - 1);
        var bottomCell = automata.GetCell(bottomPosition, false);

        if (automata.CanDisplace(currentCell, bottomCell))
        {
            if (bottomCell.IsLiquid() || bottomCell.IsGas())
            {
                automata.DisplaceFluid(bottomPosition);
            }
            automata.SwapCells(currentPosition, bottomPosition);
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

        var canFallLeft = leftPositions.Exists(pos => automata.CanDisplace(currentCell, automata.GetCell(pos, false)));
        var canFallRight = rightPositions.Exists(pos => automata.CanDisplace(currentCell, automata.GetCell(pos, false)));

        if (canFallLeft || canFallRight)
        {
            int direction = (canFallLeft ? -1 : 0) + (canFallRight ? 1 : 0);
            if (direction == 0)
            {
                direction = (Random.value > 0.5f) ? 1 : -1;
            }
            var targetPosition = direction == -1 ? leftPositions.First(pos => automata.CanDisplace(currentCell, automata.GetCell(pos, false)))
                                                 : rightPositions.First(pos => automata.CanDisplace(currentCell, automata.GetCell(pos, false)));
            automata.SwapCells(currentPosition, targetPosition);
        }
    }

    private void HandleNegativeFluidity(int absFluidity, int x, int y)
    {
        var leftBottomPosition = new Vector2Int(x - 1, y - absFluidity);
        var rightBottomPosition = new Vector2Int(x + 1, y - absFluidity);

        var leftBottomCell = automata.GetCell(leftBottomPosition, false);
        var leftBelowLeftBottomCell = automata.GetCell(new Vector2Int(x - 1, y - absFluidity - 1), false);

        var rightBottomCell = automata.GetCell(rightBottomPosition, false);
        var rightBelowRightBottomCell = automata.GetCell(new Vector2Int(x + 1, y - absFluidity - 1), false);

        bool canMoveLeft = leftBottomCell.material == CellMaterial.Empty && leftBelowLeftBottomCell.material != CellMaterial.Empty;
        bool canMoveRight = rightBottomCell.material == CellMaterial.Empty && rightBelowRightBottomCell.material != CellMaterial.Empty;

        if (canMoveLeft && canMoveRight)
        {
            var targetPosition = (Random.value > 0.5f) ? leftBottomPosition : rightBottomPosition;
            automata.SwapCells(currentPosition, targetPosition);
        }
        else if (canMoveLeft)
        {
            automata.SwapCells(currentPosition, leftBottomPosition);
        }
        else if (canMoveRight)
        {
            automata.SwapCells(currentPosition, rightBottomPosition);
        }
    }
}