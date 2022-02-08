using Godot;
using System;

public class MainMenu : Control
{
	private RichTextLabel gameTitle;

	[Export]
	public DynamicFont FontToUse { get; set; }
	public override void _Ready()
	{
		gameTitle = GetNode<RichTextLabel>("GameTitle");

		gameTitle.PushFont(FontToUse);

		var viewport = GetViewportRect();
		gameTitle.RectPosition = (viewport.Size / 2);

		gameTitle.RectPosition = new Vector2(gameTitle.RectPosition.x - (gameTitle.Text.Length() * 8), gameTitle.RectPosition.y);

	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
