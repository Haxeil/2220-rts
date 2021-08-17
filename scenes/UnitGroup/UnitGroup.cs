using Godot;
using System;

public class UnitGroup : Spatial
{
	// How should this group be organized in the game?
	public enum UnitGroupElement {
		Box
	}

	[Export] public UnitGroupElement Element = UnitGroupElement.Box;
	[Export] public float Spacing = 0.5f;
	// It is relatively safe to assume that every unit derives from Spatial.
	[Export] public Godot.Collections.Array<NodePath> Units;

	private Navigation nav;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		nav = GetParent<Navigation>();

		UpdateUnitPaths();
	}

	// Tell the UnitGroup about a new unit joining the group, to allocate them
	// a spot.
	public void RegisterUnit(NodePath path) {
		Units.Add(path);
	}

	// Tell the UnitGroup that a unit left the group.
	public void UnregisterUnit(NodePath path) {
		Units.Remove(path);
	}

	// Based upon the current position of the UnitGroup node and its element type,
	// update all of the registered nodes to move to their correct positions.
	public void UpdateUnitPaths() {
		// Imagine distributing just points around the position of this node
		// based on the element type. Then after you've distributed those points,
		// get the closest position on the navmesh to each of those, and tell
		// each unit to move there.
		if (Units.Count > 0) {
			Vector3[] positions = new Vector3[Units.Count];
			switch (Element) {
				case UnitGroupElement.Box:
					int width = Units.Count / 2; // width = (width*height) / 2
					Vector3 box_origin = new Vector3(-width - Spacing, Transform.origin.y, -width - Spacing);
					for (int row = 0; row < width; ++row) {
						for (int col = 0; col < width; ++col) {
							int idx = row * col; // NOTE: this is not going to work. my brain hurts at midnight and I can't do math. Make the box!
							//positions[v].x = box_origin 
						}
					}
					break;
			}
		}
	}
}
