using Godot;
using System;

public class Cam : Spatial
{
	[Export] private int moveMargin = 20;
	[Export] private int moveSpeed = 30;

	[Export] private int rayLength = 1000;

	private Camera camera;
	private Godot.Collections.Array<Unit> SelectedUnits = new Godot.Collections.Array<Unit>();
	private SelectionBox selectionBox;
	private Vector2 startSelPos;
	private PackedScene unit_group;
	private PackedScene positionArrowScene;

	public override void _Ready()
	{
		camera = GetChild<Camera>(0);
		selectionBox = GetNode<SelectionBox>("SelectionBox");
		unit_group = ResourceLoader.Load<PackedScene>("res://scenes/UnitGroup/UnitGroup.tscn");
		positionArrowScene = ResourceLoader.Load<PackedScene>("res://scenes/Cam/PositionArrows.tscn");
	}

	public override void _Process(float delta) {
		var mousePos = GetViewport().GetMousePosition();
		CalcMove(mousePos, delta);

		if (Input.IsActionJustPressed("main_command")) {
			MoveSelectedUnits(mousePos);
		}
		if (Input.IsActionJustPressed("alt_command")) {
			selectionBox.startBoxPoint = mousePos;
			startSelPos = mousePos;
		}

		if (Input.IsActionPressed("alt_command")) {
			selectionBox.mousePos =  mousePos;
			selectionBox.isVisible = true;
		} else {
			selectionBox.isVisible = false;
		}

		if (Input.IsActionJustReleased("alt_command")) {
			SelectUnits(mousePos);
		}

	}
	private void CalcMove(Vector2 mousePos, float delta) {
		var vSize = GetViewport().Size;

		var moveVec = Vector3.Zero;
		if (mousePos.x < moveMargin || Input.IsActionPressed("move_left")) {
			moveVec.x -= 1;
		}
		if (mousePos.y < moveMargin || Input.IsActionPressed("move_up")) {
			moveVec.z -= 1;
		}
		if (mousePos.x > vSize.x - moveMargin || Input.IsActionPressed("move_right")) {
			moveVec.x += 1;
		}
		if (mousePos.y > vSize.x - moveMargin || Input.IsActionPressed("move_down")) {
			moveVec.z += 1;
		}
		moveVec = moveVec.Rotated(Vector3.Up, this.RotationDegrees.y);
		GlobalTranslate(moveVec * moveSpeed * delta);
	}


	private void MoveSelectedUnits(Vector2 mousePos) {
		var result = RayCastFromMouse(mousePos, 1);
		if (result == null) {
			return;
		}
		if (result["position"] is Vector3 position) {
			if (SelectedUnits.Count > 0) {
				PointToDirection(position);
				if (SelectedUnits.Count > 1) {
					// If more than one units selected are told to move,
					// we create a UnitGroup and add them to it.
					UnitGroup ug = unit_group.Instance<UnitGroup>();
					GetTree().Root.GetNode("World/Navigation").AddChild(ug, true);
					ug.Translation = position;

					foreach (var unit in SelectedUnits) {
						unit.UnitGroup = ug; // Setter of property calls RegisterUnit method of ug
					}
					ug.UpdateUnitPaths();
				} else {
					SelectedUnits[0].MoveTo(position);
					
				}
			}
		}
	}

	private void SelectUnits(Vector2 mousePos) {
		var newSelectedUnits = new Godot.Collections.Array<Unit>();
		if (mousePos.DistanceSquaredTo(startSelPos) < 16) {
			var u = GetUnitUnderMouse(mousePos);
			if (u != null) {
				newSelectedUnits.Add(u);
			}
		} else {
			newSelectedUnits = GetUnits(startSelPos, mousePos);
		}

		if (newSelectedUnits.Count != 0) {
			GD.Print("ne w selected", newSelectedUnits.Count);
			for (int indx = 0; indx < SelectedUnits.Count; indx++) {
				GD.Print("selected : ", SelectedUnits.Count, "indx : ", indx);
				SelectedUnits[indx].Deselect();
			}
			for (int indx = 0; indx < newSelectedUnits.Count; indx++) {
				newSelectedUnits[indx].Select();
			}

			SelectedUnits = newSelectedUnits;

		}
	}

	private Unit GetUnitUnderMouse(Vector2 mousePos) {
		var result = RayCastFromMouse(mousePos, 3);
		if (result == null) {
			return null;
		}
		if (result["collider"] is Unit collider) {
			return collider;
		} else {
			return null;
		}
	}
	//select units inside the Box
	private Godot.Collections.Array<Unit> GetUnits(Vector2 topLeft, Vector2 botRight) {

		
		if (topLeft.x > botRight.x) {
			var temp = topLeft.x;
			topLeft.x = botRight.x;
			botRight.x = temp;
		}

		if (topLeft.y > botRight.y) {
			var temp = topLeft.y;
			topLeft.y = botRight.y;
			botRight.y = temp;
		}
		var rect = new Rect2(topLeft, botRight - topLeft);
		Godot.Collections.Array<Unit> SelectedUnits = new Godot.Collections.Array<Unit>();

        	var soldier_list = GetTree().GetNodesInGroup("HumanSoldier");
        	for (int indx = 0; indx < soldier_list.Count; indx++) {
            		var soldier = soldier_list[indx] as Unit;
            		if (rect.HasPoint(camera.UnprojectPosition(soldier.GlobalTransform.origin))) {
				SelectedUnits.Add(soldier);
			}
		}

		return SelectedUnits;
	}

	private Godot.Collections.Dictionary RayCastFromMouse(Vector2 mousePos, uint collisionMask) {
		var rayStart = camera.ProjectRayOrigin(mousePos);
		var rayEnd = rayStart + camera.ProjectRayNormal(mousePos) * rayLength;

		var spaceState = GetWorld().DirectSpaceState;
		return spaceState.IntersectRay(rayStart, rayEnd, new Godot.Collections.Array {}, collisionMask);
	}

	private void PointToDirection(Vector3 position) {
		var positionArrow = positionArrowScene.Instance<Spatial>();
		this.GetParent().AddChild(positionArrow);
		positionArrow.GlobalTranslate(new Vector3(position.x, 0, position.z));
	}
}
