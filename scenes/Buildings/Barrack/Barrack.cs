using Godot;
using System;

public class Barrack : Building
{
	// dictionary when exported it dosn't show the types assigned here.
	//[Export] private Godot.Collections.Dictionary<Enums.HumanUnits, Godot.Collections.Array<Texture>> UnitTextures;
	private PackedScene soldierScene;
	public override void _Ready()
	{
		base._Ready();
		soldierScene = ResourceLoader.Load<PackedScene>("res://scenes/Units/HumanUnits/HumanSoldier.tscn");
		// The units the building can produce.
		unitData.Add(Enums.Units.Soldier, unitTextures);
	}

	// Method Called From the Btn to spawn Soldiers.
	protected override void SpawnUnit(Enums.Units unit)
	{
		var pos = GetNode<Position3D>("SpawnPoint").GlobalTransform.origin;
		Action<Unit> SetUnit = u => {
			//u.GlobalTranslate(pos);
			u.Translation = pos;
				// Navigation Node
			GetParent().GetParent().AddChild(u);
		};

		switch (unit)
		{
			case Enums.Units.Soldier :
				var soldier = soldierScene.Instance<HumanSoldier>();
				SetUnit(soldier);
				break;

		}


	}
	public override void Select() {
		GetNode<MeshInstance>("Selected").Show();
		UpdateMainPanel();
	}

	public override void Deselect() {
		GetNode<MeshInstance>("Selected").Hide();
	}
	// Method called whem this building is selected.
	protected override void UpdateMainPanel()
	{
		// Send Textures to the UI to display them.
		GetTree().CallGroup("MainPanel", "DisplayTextures", unitData, groupName);
	}

}
