using Godot;
using System;

public class HumanSoldier : KinematicBody
{
	[Export] private Vector3[] path;
	private int pathIndx = 0;
	[Export] private float speed = 12f;
	private Navigation nav;
	private Spatial body;
	public override void _Ready()
	{
		nav = GetParent<Navigation>();
		body = GetNode<Spatial>("Body");
		
	}

	//called from Cam script 
	private void MoveTo(Vector3 targetPos) {
		path = nav.GetSimplePath(this.GlobalTransform.origin, targetPos);
		pathIndx = 0;
	}

	public override void _PhysicsProcess(float _delta)
	{
		if (path == null) {
			//GD.Print("Null");
			return;
		}

		if (pathIndx < path.Length) {
			var direction = (path[pathIndx] - this.GlobalTransform.origin);
			if (direction.Length() < 0.1) {
				pathIndx += 1;
			} else {
				

				
				var velocity = this.MoveAndSlide(direction.Normalized() * speed, Vector3.Up, false, 4, Mathf.Pi/4, false);
				//lerp_angle($Mesh.rotation.y, atan2(move_vec.x, move_vec.z), delta * ang_accel)
				GetNode<AnimationPlayer>("Animation").Play("Walking");
				if (Vector3.Up.Cross((Transform.origin - velocity ) - GlobalTransform.origin) != new Vector3()) {
					LookAt(Transform.origin - velocity, Vector3.Up);

				}



				
				
			}
		}
	}

}
