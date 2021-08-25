using Godot;
using System;

public abstract class Building : StaticBody, ISelectable
{


	// Determines who controls this unit, and who this unit chooses to engage.
	[Export] public Enums.Faction _Faction;
	// when selected send Barrack Info to the UI.
	// Brracks are for soldier so the UI should show soldiers 
    [Export] protected Godot.Collections.Array<Texture> unitTextures;

    [Export] protected Godot.Collections.Dictionary<Enums.Units, Godot.Collections.Array<Texture>> unitData = new Godot.Collections.Dictionary<Enums.Units, Godot.Collections.Array<Texture>> {};

    protected String groupName = "Building";


	// Called When player clicks on the unit he wants to produce.
    protected abstract void SpawnUnit(Enums.Units unit);
    // Called when the building selected to update the UI.
    protected abstract void UpdateMainPanel();
    	// Called when the player has selected this Unit. Make the selection visible.
	public abstract void Select();
	// Called when the player has deselected this Unit.
	public abstract void Deselect();
    public override void _Ready()
    {
        groupName = GetGroups()[0].ToString();
        
    }
  

}