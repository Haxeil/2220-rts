using Godot;
using System;

public class Btn : TextureButton
{
	// To define what unit can the btn produce.
	// TODO : Every btn should have his unique unit.
	[Export] public Enums.Units unit = Enums.Units.None;
	public String building = "";
	private void _on_C0R0_pressed()
	{
		GetTree().CallGroup(building, "SpawnUnit", unit);
	}


	private void _on_C0R1_pressed()
	{
		GetTree().CallGroup(building, "SpawnUnit", unit);
	}


	private void _on_C0R2_pressed()
	{
		GetTree().CallGroup(building, "SpawnUnit", unit);
	}


	private void _on_C0R3_pressed()
	{
		GetTree().CallGroup(building, "SpawnUnit", unit);
	}

}



