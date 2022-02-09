using Godot;
using System;

public class MainMenu : Control
{
	private Label gameTitle;

	[Export]
	public DynamicFont FontToUse { get; set; }

	public override void _Ready()
	{
		gameTitle = GetNode<Label>("GameTitle");
		FontToUse.Size = 64;
		FontToUse.OutlineSize = 5;
		FontToUse.OutlineColor = new Color(0, 0, 0, 1);

		FontToUse.UseFilter = true;
		gameTitle.AddFontOverride("font",FontToUse);

		var viewport = GetViewportRect();
		gameTitle.RectPosition = (viewport.Size / 2);

		gameTitle.RectPosition = new Vector2(gameTitle.RectPosition.x - (gameTitle.Text.Length() * 16), gameTitle.RectPosition.y);

	}
}
