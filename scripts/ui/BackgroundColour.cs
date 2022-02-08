using Godot;
using System;

public class BackgroundColour : ColorRect
{
	[Export]
	public Vector3 Colour { get; set; } = Vector3.One;

	public override void _Ready()
	{
		MatchViewportSize();
	}

	private void MatchViewportSize()
	{
		var viewportSize = GetViewportRect();

		RectSize = viewportSize.Size;
	}
}
