using Godot;
using System;

public class Cam : Spatial
{
	[Export] private Enums.Faction controlling_faction = Enums.Faction.Human;

	[Export] private int moveMargin = 20;
	[Export] private int moveSpeed = 30;

	[Export] private int rayLength = 1000;

	private Camera camera;
	private Godot.Collections.Array<Unit> selectedUnits = new Godot.Collections.Array<Unit>();
	
	private Godot.Collections.Array<Building> selectedBuildings = new Godot.Collections.Array<Building>();
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
		// Left Mouse btn 0.
		if (Input.IsActionJustPressed("main_command")) {
			MoveSelectedUnits(mousePos);
		}
		// Right Mouse btn 1.
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
			SelectObjs(mousePos);
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
		GD.Print(selectedUnits);
		var result = RayCastFromMouse(mousePos, 1);
		if (result == null) {
			return;
		}
		if (result["position"] is Vector3 position) {
			if (selectedUnits.Count > 0) {
				PointToDirection(position);
				if (selectedUnits.Count > 1) {
					// If more than one units selected are told to move,
					// we create a UnitGroup and add them to it.
					UnitGroup ug = unit_group.Instance<UnitGroup>();
					GetTree().Root.GetNode("World/Navigation").AddChild(ug, true);
					ug.Translation = position;

					foreach (var unit in selectedUnits) {
						unit.UnitGroup = ug; // Setter of property calls RegisterUnit method of ug
					}
					ug.UpdateUnitPaths();
					
				} else {
					selectedUnits[0].MoveTo(position);
					
				}
			}
		}
	}

	private void SelectObjs(Vector2 mousePos) {
		var newSelectedUnits = new Godot.Collections.Array<Unit>();
		var newSelectedBuildings = new Godot.Collections.Array<Building>();

		if (mousePos.DistanceSquaredTo(startSelPos) < 16) {
			var u = GetObjMouse(mousePos);
			
			if (u != null) {
				if (u is Unit) {					
					newSelectedUnits.Add(u as Unit);
				} else {
					newSelectedBuildings.Add(u as Building);
				}
			} else {
				DeselectSelectedUnits();
				DeselectSelectedBuildings();
			}
		} else {
			(newSelectedUnits, newSelectedBuildings) = GetObjs(startSelPos, mousePos);
		}

		if (newSelectedUnits.Count != 0) {
			GD.Print("new selected", newSelectedUnits.Count);
			for (int indx = 0; indx < selectedUnits.Count; indx++) {
				selectedUnits[indx].Deselect();
			}
			for (int indx = 0; indx < newSelectedUnits.Count; indx++) {
				newSelectedUnits[indx].Select();
			}

			selectedUnits = newSelectedUnits;

		} else {
			DeselectSelectedUnits();
		}
		if (newSelectedBuildings.Count != 0) {
			GD.Print("new selected", newSelectedBuildings.Count);
			for (int indx = 0; indx < newSelectedBuildings.Count; indx++) {
				newSelectedBuildings[indx].Deselect();
			}
			for (int indx = 0; indx < newSelectedBuildings.Count; indx++) {
				newSelectedBuildings[indx].Select();
			}

			selectedBuildings = newSelectedBuildings;

		} else {
			DeselectSelectedBuildings();
		}

	}
	// Obj could only be Unit or Building.
	private ISelectable GetObjMouse(Vector2 mousePos) {
		var result = RayCastFromMouse(mousePos, 3);
		if (result == null) {
			return null;
		}
		if (result["collider"] is ISelectable collider) {
			return collider;
		} else {
			return null;
		}
	}
	//select units inside the Box
	private (Godot.Collections.Array<Unit>, Godot.Collections.Array<Building>) GetObjs(Vector2 topLeft, Vector2 botRight) {

		
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
		Godot.Collections.Array<Unit> selectedUnits = new Godot.Collections.Array<Unit>();
		Godot.Collections.Array<Building> selectedBuildings = new Godot.Collections.Array<Building>();

		var unit_list = GetTree().GetNodesInGroup("Unit");
		foreach (var node in unit_list) {
					//var soldier = soldier_list[indx] as Unit;
			if (node is Unit unit) {
				if (unit._Faction == controlling_faction) {
							if (rect.HasPoint(camera.UnprojectPosition(unit.GlobalTransform.origin))) {
						selectedUnits.Add(unit);
					}
				}
			} else {
				GD.PushWarning($"node {node.ToString()} is not of type Unit or is null");
			}
		}
		var building_list = GetTree().GetNodesInGroup("Building");
		foreach (var node in building_list) {
					//var soldier = soldier_list[indx] as Unit;
			if (node is Building building) {
				if (building._Faction == controlling_faction) {
							if (rect.HasPoint(camera.UnprojectPosition(building.GlobalTransform.origin))) {
						selectedBuildings.Add(building);
					}
				}
			} else {
				GD.PushWarning($"node {node.ToString()} is not of type Unit or is null");
			}
		}
		return (selectedUnits, selectedBuildings);
	}

	private Godot.Collections.Dictionary RayCastFromMouse(Vector2 mousePos, uint collisionMask) {
		var rayStart = camera.ProjectRayOrigin(mousePos);
		var rayEnd = rayStart + camera.ProjectRayNormal(mousePos) * rayLength;

		var spaceState = GetWorld().DirectSpaceState;
		return spaceState.IntersectRay(rayStart, rayEnd, new Godot.Collections.Array {}, collisionMask);
	}
	// Called when the player wants to deselect current selected units 
	// when click on the ground or selecting empty area.
	private void DeselectSelectedUnits() {
		if (selectedUnits.Count != 0) {
			foreach (var unit in selectedUnits) {
				unit.Deselect();
			}
			selectedUnits.Clear();
		}

	}

	private void DeselectSelectedBuildings() {
		if (selectedBuildings.Count != 0) {
			foreach (var building in selectedBuildings) {
				building.Deselect();
			}
			selectedBuildings.Clear();
		}
		GetTree().CallGroup("MainPanel", "ClearPanel");
	}

	private void PointToDirection(Vector3 position) {
		var positionArrow = positionArrowScene.Instance<Spatial>();
		this.GetParent().AddChild(positionArrow);
		positionArrow.GlobalTranslate(new Vector3(position.x, 0, position.z));
	}

	
	public void ToggleProcess(bool toggle){
		SetProcess(toggle);
	}
}



