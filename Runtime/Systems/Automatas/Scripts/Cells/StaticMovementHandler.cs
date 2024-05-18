public class StaticMovementHandler : MovementHandler
{
    public StaticMovementHandler(CellularAutomata automata) : base(automata) { }

    public override void Move() { /* No movement for static cells */ }
}
