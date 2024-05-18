
using UnityEngine;

public class LiquidMovementHandler : MovementHandler
{
    public LiquidMovementHandler(CellularAutomata automata) : base(automata) { }

    public override void Move()
    {
        int x = currentPosition.x;
        int y = currentPosition.y;
        var bottomPosition = new Vector2Int(x, y - 1);
        var bottomCell = automata.GetCell(bottomPosition);

        if (automata.CanDisplace(currentCell, bottomCell))
        {
            automata.SwapCells(currentPosition, bottomPosition);
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
                var leftCell = automata.GetCell(leftPosition, false);
                var leftBelowPosition = currentPosition + new Vector2Int(-i, -1);
                var leftBelowCell = automata.GetCell(leftBelowPosition, false);

                if (leftCell.IsSolid())
                {
                    break; // Stop if we encounter a solid cell
                }

                if (automata.CanDisplace(currentCell, leftBelowCell))
                {
                    leftSuccess = leftBelowCell;
                    leftSuccessPosition = leftBelowPosition;
                    break;
                }
                else if (!automata.CanDisplace(currentCell, leftCell))
                {
                    break;
                }
            }
            if (leftSuccess == null)
            {
                for (int i = 1; i < fluidity; i++)
                {
                    var leftPosition = currentPosition + new Vector2Int(-i, 0);
                    var leftCell = automata.GetCell(leftPosition, false);
                    if (leftCell.IsSolid())
                    {
                        break; // Stop if we encounter a solid cell
                    }

                    if (automata.CanDisplace(currentCell, leftCell))
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
                var rightCell = automata.GetCell(rightPosition, false);
                var rightBelowPosition = currentPosition + new Vector2Int(i, -1);
                var rightBelowCell = automata.GetCell(rightBelowPosition, false);

                if (rightCell.IsSolid())
                {
                    break; // Stop if we encounter a solid cell
                }

                if (automata.CanDisplace(currentCell, rightBelowCell))
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
                    var rightCell = automata.GetCell(rightPosition, false);
                    if (rightCell.IsSolid())
                    {
                        break; // Stop if we encounter a solid cell
                    }

                    if (automata.CanDisplace(currentCell, rightCell))
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
                    automata.SwapCells(currentPosition, leftSuccessPosition);
                }
                else
                {
                    automata.SwapCells(currentPosition, rightSuccessPosition);
                }
            }
        }
    }
}