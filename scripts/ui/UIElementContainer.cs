using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;
using System;
using System.Text;

namespace FaffLatest.scripts.ui
{ 
	public class UIElementContainer : Control
	{
		public CharacterSelector CharacterSelector { get; private set; }

		public SelectedCharacter SelectedCharacter { get; private set; }

		public HealthBar HealthBar { get; private set; }

		public Control EffectLabelsParent { get; private set; }

		public ConfirmationDialog ConfirmationDialog {get; private set;}

		public override void _Ready()
		{
			GetUIChildrenNodes();
			ConnectSignals();

			var viewportSize = GetViewport().GetVisibleRect().Size;
			SelectedCharacter.RectPosition = new Vector2(viewportSize.x - 200, viewportSize.y - 250);
			base._Ready();
		}

		private void GetUIChildrenNodes()
		{
			CharacterSelector = GetNode<CharacterSelector>("CharacterSelector");
			ConfirmationDialog = GetNode<ConfirmationDialog>("ConfirmationDialog");
			SelectedCharacter = GetNode<SelectedCharacter>("SelectedCharacter");
			EffectLabelsParent = GetNode<Control>("EffectLabels");
		}

		private void ConnectSignals()
		{
			SelectedCharacter.ConnectSignals(GetNode(NodeReferences.Systems.GAMESTATE_MANAGER));
		}
	}
}
