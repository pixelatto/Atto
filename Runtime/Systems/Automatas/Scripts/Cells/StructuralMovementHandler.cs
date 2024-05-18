
public class StructuralMovementHandler : GranularMovementHandler
{
    public StructuralMovementHandler(CellularAutomata automata) : base(automata) { }

    public override void Move()
    {
        if (currentCell.needsUpdate)
        {
            automata.UpdateStructuralConnections(currentPosition);
        }

        if (currentCell.isStructurallyConnected)
        {
            return;
        }
        else
        {
            base.Move();
        }
    }
}
