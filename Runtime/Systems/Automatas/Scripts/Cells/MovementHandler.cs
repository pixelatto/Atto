using UnityEngine;

public abstract class MovementHandler
{
    protected CellularAutomata automata;
    protected Vector2Int currentPosition;
    protected Cell currentCell;

    public MovementHandler(CellularAutomata automata)
    {
        this.automata = automata;
    }

    public void SetCurrentState(Vector2Int position, Cell cell)
    {
        currentPosition = position;
        currentCell = cell;
    }

    public abstract void Move();
}