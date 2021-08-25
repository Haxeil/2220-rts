using Godot;
using System;
public class MainPanel : Control
{

	private TextureRect BoxSelection;
	private int tableIndx = 0;
	

	public override void _Ready()
	{
		BoxSelection = GetNode<TextureRect>("BoxSelection");
	}
	// Called From Buildings, when they are selected.
	public void DisplayTextures(Godot.Collections.Dictionary<Enums.Units, Godot.Collections.Array<Texture>> unitData, String groupName) {
		foreach (var uD in unitData) {
			for (var idx = 0; idx < BoxSelection.GetChildCount(); idx++) {
				var btn = BoxSelection.GetChild<Btn>(idx);
				// if the unit already registered 
				// break.
				if (btn.unit == uD.Key) {
					break;
				}
				if (btn.unit == Enums.Units.None) {
					btn.unit = uD.Key;
					btn.building = groupName;
					btn.TextureNormal = uD.Value[0];
					btn.TexturePressed = uD.Value[1];
					btn.TextureHover = uD.Value[2];
					btn.TextureDisabled = uD.Value[3];
					break;
				}
			}
		}

	}





}


