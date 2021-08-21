using Godot;

public class UnitState : Node {

    [Export] public Enums.UnitState currentState;

	[Export] public Enums.UnitState nextState;

	[Export] public Enums.UnitState prevState;
}