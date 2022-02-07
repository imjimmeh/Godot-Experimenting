using FaffLatest.scripts.characters;
using Godot;
using System;

namespace FaffLatest.scripts.ui
{ 
	public class UIElementContainer : Node
	{
		private TextureRect characterFaceIcon;
		public TextureRect CharacterFaceIcon => characterFaceIcon;

		public override void _Ready()
		{
			base._Ready();

			characterFaceIcon = GetNode<TextureRect>("CharacterIcon");

			var viewportSize = GetViewport().GetVisibleRect().Size;

			characterFaceIcon.RectPosition = new Vector2(viewportSize.x - 148, viewportSize.y - 148);
		}

	}
}
