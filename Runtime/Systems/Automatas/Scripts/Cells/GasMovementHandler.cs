
using System.Collections.Generic;
using UnityEngine;

public class GasMovementHandler : MovementHandler
{
    public GasMovementHandler(CellularAutomata automata) : base(automata) { }

    public override void Move()
    {
        int x = currentPosition.x;
        int y = currentPosition.y;
        var targetPositions = new List<Vector2Int>();
        var fluidity = currentCell.fluidity;

        for (int i = -fluidity; i <= fluidity; i++)
        {
            for (int j = -fluidity; j <= fluidity; j++)
            {
                var targetPosition = new Vector2Int(x + i, y + j - currentCell.gravity);
                var targetCell = automata.GetCell(targetPosition);

                if (automata.CanDisplace(currentCell, targetCell))
                {
                    targetPositions.Add(targetPosition);
                }
            }
        }

        if (targetPositions.Count > 0)
        {
            var randomPosition = targetPositions.PickRandom();
            var randomCell = automata.GetCell(randomPosition);
            if (randomCell.IsLiquid() || randomCell.IsGas())
            {
                automata.DisplaceFluid(randomPosition);
            }
            automata.SwapCells(currentPosition, randomPosition);
        }
    }
}