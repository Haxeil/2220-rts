using Godot;
using System;

public class SelectionBox : Control
{
	public bool isVisible = true;
	public Vector2 mousePos = Vector2.Zero;
	//the starting position of the Box;
	public Vector2 startBoxPoint = Vector2.Zero;
	// Color of the Box;
	[Export] Color boxColor = new Color(0, 1, 0);
	[Export] int boxEdgeWidth = 3;


	public override void _Draw()
	{
		if (isVisible && startBoxPoint != mousePos) {
			DrawLine(startBoxPoint, new Vector2(mousePos.x, startBoxPoint.y), boxColor, boxEdgeWidth);
			DrawLine(startBoxPoint, new Vector2(startBoxPoint.x, mousePos.y), boxColor, boxEdgeWidth);
			DrawLine(mousePos, new Vector2(mousePos.x, startBoxPoint.y), boxColor, boxEdgeWidth);
			DrawLine(mousePos, new Vector2(startBoxPoint.x, mousePos.y), boxColor, boxEdgeWidth);
		}
	}
	public override void _Process(float delta)
	{
		Update();
	}

}
