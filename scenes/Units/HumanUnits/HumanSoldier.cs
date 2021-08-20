using Godot;
using System;

public class HumanSoldier : Unit
{
	[Export] private Vector3[] path;
	private int pathIndx = 0;
	[Export] private float speed = 12f;
	//private Navigation nav;
	private Spatial character;

	[Export] private float angular_velocity = 7f;
	public override void _Ready()
	{
		base._Ready(); // Call Unit._Ready() to get the Nav
		character = GetNode<Spatial>("AnimatedCharacter");
		
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
			} else {
				
				var velocity = this.MoveAndSlide(direction.Normalized() * speed, Vector3.Up, false, 4, Mathf.Pi/4, false);

				character.GetNode<AnimationPlayer>("AnimationPlayer").Play("Walking");
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
}



