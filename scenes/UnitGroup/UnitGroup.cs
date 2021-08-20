using Godot;
using System;

public class UnitGroup : Spatial
{
	const float Spacing = 2f;

	// It is relatively safe to assume that every unit derives from Spatial.
	private Godot.Collections.Array<Unit> units;

	private Navigation nav;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		units = new Godot.Collections.Array<Unit>();
		nav = GetParent<Navigation>();

		UpdateUnitPaths();
	}

	// This function will be called by Unit.UnitGroup Property.
	public void RegisterUnit(Unit unit) {
		units.Add(unit);
	}

	// This function will be called by Unit.UnitGroup Property.
	public void UnregisterUnit(Unit unit) {
		units.Remove(unit);
		if (units.Count <= 0) QueueFree(); // Delete self when not the owner of any units
	}

	// Based upon the current position of the UnitGroup node and its element type,
	// update all of the registered nodes to move to their correct positions.
	public void UpdateUnitPaths() {
		// Imagine distributing just points around the position of this node.
		// Then after you've distributed those points, get the closest position
		// on the navmesh to each of those, and tell each unit to move there.
		if (units.Count > 0) {
			int width = Math.Max(2, units.Count / 2); // width = (width*height) / 2
			float half_width = width * .5f;
			Vector3 origin = new Vector3(
					Transform.origin.x - half_width - (Spacing*half_width),
					Transform.origin.y,
					Transform.origin.z - half_width - (Spacing*half_width));

			for (int row = 0; row < width; ++row) {
				for (int col = 0; col < width; ++col) {
					int idx = (row * width) + col;
					if (idx >= units.Count) return;
					units[idx].MoveTo(new Vector3(
							origin.x + col*(width+Spacing),
							origin.y,
							origin.z + row*(width+Spacing)));
				}
			}
		}
	}
}
