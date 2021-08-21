using Godot;
using System;

public abstract class Unit : KinematicBody
{
	public enum Faction {
		Human,
		Alien
	}

	// Determines who controls this unit, and who this unit chooses to engage.
	[Export] public Faction _Faction;


	private UnitGroup unit_group = null; // The group of the unit, null otherwise
	public UnitGroup UnitGroup
	{
		get { return unit_group; }
		set {
			// Null checks
			if (unit_group is UnitGroup) {
				unit_group.UnregisterUnit(this);
			}
			if (value is UnitGroup) {
				value.RegisterUnit(this);
			}
			unit_group = value;
		}
	}

	private NodePath nav_node_path = "..";
	public Navigation Nav { get; private set; }
	// Set NavNodePath to change Navigation agent used for this Unit.
	[Export] public NodePath NavNodePath {
		get { return nav_node_path; }
		set {
			nav_node_path = value;
			Nav = GetNode<Navigation>(value);
		}
	}
	// Called When the Unit State changes.
	[Signal] protected delegate void StateChanged(Enums.UnitState s);
	public UnitState state = new UnitState();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Nav = GetNode<Navigation>(nav_node_path);
	}

	// MoveTo is implemented in classes that derive from Unit.
	public abstract void MoveTo(Vector3 targetPos);

	// Called when the player has selected this Unit. Make the selection visible.
	public abstract void Select();
	// Called when the player has deselected this Unit.
	public abstract void Deselect();
	// Called when the state changes to play an animation.
	protected abstract void PlayAnimation();

	// Called when a unit state changes;
	public void _On_UnitStateChanged(Enums.UnitState s)
	{
		// if the current gameplay state is DEAD dont change the state.
		if (state.currentState == Enums.UnitState.DEAD) {return;}


		state.nextState = s; //IDLE // MOVE // ATTACK // DIE 

		// dont change the prevState if the nextState = the currentState
		if (state.nextState != state.currentState)
		{
			state.prevState = state.currentState; 
		}
		state.currentState = state.nextState; 
		PlayAnimation();

	}
}

