using FaffLatest.scripts.characters;
using Godot;
using System;

namespace FaffLatest.scripts.ui
{ 
	public class UIElementContainer : Node
	{
		private Control selectedCharacterPart;

		private TextureRect characterFaceIcon;
		public TextureRect CharacterFaceIcon {get =>characterFaceIcon; private set => characterFaceIcon = value;}

		public Label CharacterName;

		public CharacterSelector CharacterSelector { get; private set; }
		public override void _Ready()
		{
			selectedCharacterPart = GetNode<Control>("SelectedCharacter");

			var viewportSize = GetViewport().GetVisibleRect().Size;

			selectedCharacterPart.RectPosition = new Vector2(viewportSize.x - 200, viewportSize.y - 200);

			CharacterFaceIcon = selectedCharacterPart.	GetNode<TextureRect>("CharacterIcon");
			CharacterName = CharacterFaceIcon.GetNode<Label>("Name");

			CharacterSelector = GetNode<CharacterSelector>("CharacterSelector");

			base._Ready();
		}

	}
}
