using Godot;
using System;
public class MainPanel : Control
{

	private HBoxContainer BoxSelection;
	private int tableIndx = 0;
	
	public override void _Ready()
	{
		BoxSelection = GetNode<HBoxContainer>("BoxSelection/HContainer");
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
	// Called From Cam to clear btns when building 
	// is deselected.
	public void ClearPanel() {
		for (var idx = 0; idx < BoxSelection.GetChildCount(); idx++) {
			var btn = BoxSelection.GetChild<Btn>(idx);
			if (btn.unit != Enums.Units.None) {
				btn.unit = Enums.Units.None;
				btn.building = "";
				btn.TextureNormal = null;
				btn.TexturePressed = null;
				btn.TextureHover = null;
				btn.TextureDisabled = null;
				
			}
		}
	}

	private void _on_btn_mouse_entered()
	{
		GetParent().GetNode<Cam>("Cam").ToggleProcess(false);
	}
	private void _on_btn_mouse_exited()
	{
		GetParent().GetNode<Cam>("Cam").ToggleProcess(true);
	}


}






















