using Godot;
using System;

public class Cam : Spatial
{
	[Export] private int moveMargin = 20;
	[Export] private int moveSpeed = 30;

	[Export] private int rayLength = 1000;

	private Camera camera;

	public override void _Ready()
	{
		camera = GetChild<Camera>(0);
	}

	public override void _Process(float delta) {
		var mousePos = GetViewport().GetMousePosition();
		CalcMove(mousePos, delta);

		if (Input.IsActionJustPressed("main_command")) {
			MoveAllUnits(mousePos);
		}

	}
	private void CalcMove(Vector2 mousePos, float delta) {
		var vSize = GetViewport().Size;

		var moveVec = Vector3.Zero;
		if (mousePos.x < moveMargin) {
			moveVec.x -= 1;
		}
		if (mousePos.y < moveMargin) {
			moveVec.z -= 1;
		}
		if (mousePos.x > vSize.x - moveMargin) {
			moveVec.x += 1;
		}
		if (mousePos.y > vSize.x - moveMargin) {
			moveVec.z += 1;
		}
		moveVec = moveVec.Rotated(Vector3.Up, this.RotationDegrees.y);
		GlobalTranslate(moveVec * moveSpeed * delta);
	}

	private void MoveAllUnits(Vector2 mousePos) {
		var result = RayCastFromMouse(mousePos, 1);
		if (result == null || result.Count == 0) {
			return;
		}
		
		if (result["position"] is Vector3 position) {
			GetTree().CallGroup("HumanSoldier", "MoveTo", position);

		}


	}
	private Godot.Collections.Dictionary RayCastFromMouse(Vector2 mousePos, uint collisionMask) {
		var rayStart = camera.ProjectRayOrigin(mousePos);
		var rayEnd = rayStart + camera.ProjectRayNormal(mousePos) * rayLength;

		var spaceState = GetWorld().DirectSpaceState;
		return spaceState.IntersectRay(rayStart, rayEnd, new Godot.Collections.Array {}, collisionMask);
	}
}
