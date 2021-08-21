using Godot;
using System;


public class HumanSoldier : Unit
{
	[Export] private Vector3[] path;
	private int pathIndx = 0;
	[Export] private float speed = 12f;
	//private Navigation nav;
	private Spatial character;
	private AnimationPlayer animPlayer;

	[Export] private float angular_velocity = 7f;

	public override void _Ready()
	{
		base._Ready(); // Call Unit._Ready() to get the Nav
		character = GetNode<Spatial>("AnimatedCharacter");
		animPlayer = character.GetNode<AnimationPlayer>("AnimationPlayer");
	}

	//called from Cam script 
	public override void MoveTo(Vector3 targetPos) {
		path = Nav.GetSimplePath(this.GlobalTransform.origin, targetPos);
		pathIndx = 0;
	}

	public override void _PhysicsProcess(float delta)
	{
		
		
		if (path == null) {
			return;
		}

		if (pathIndx < path.Length) {
			var direction = (path[pathIndx] - this.GlobalTransform.origin);
			if (direction.Length() < 0.1) {
				pathIndx += 1;
				EmitSignal("StateChanged", Enums.UnitState.IDLE);
			} else {
				
				var velocity = this.MoveAndSlide(direction.Normalized() * speed, Vector3.Up, false, 4, Mathf.Pi/4, false);
				// Emit the signal to change the state
				EmitSignal("StateChanged", Enums.UnitState.MOVE);
				// change in y angle 
				var deltaYAngle = Mathf.LerpAngle(Rotation.y, Mathf.Atan2(velocity.x, velocity.z), delta * angular_velocity);

				Rotation = new Vector3(0, deltaYAngle, 0);

				
			}
		}
	}


	public override void Select() {
		GetNode<MeshInstance>("Selected").Show();
	}

	public override void Deselect() {
		GetNode<MeshInstance>("Selected").Hide();
	}


	protected override void PlayAnimation()
	{

		switch (state.currentState)
		{
			case Enums.UnitState.IDLE:
				animPlayer.Play("IDLE");
				break;
			case Enums.UnitState.MOVE:
				animPlayer.Play("MOVE");
				break;
			case Enums.UnitState.ATTACK:
				animPlayer.Play("ATTACK");
				break;
			case Enums.UnitState.RELOAD:
				animPlayer.Play("RELOAD");
				break;
			case Enums.UnitState.DEAD:
				animPlayer.Play("DEAD");
				break;
		}
	}
}
