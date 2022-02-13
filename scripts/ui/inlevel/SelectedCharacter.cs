using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.ui
{
	public class SelectedCharacter : Panel
	{
		private TextureRect faceIcon;
		private HealthBar healthbar;
		private Label name;

		private Character selectedCharacter;

		public override void _Ready()
		{
			GetChildrenNodes();
			base._Ready();
		}

		public void ConnectSignals(Node gameStateManager)
		{
			GD.Print("Connected signals");
			gameStateManager.Connect(SignalNames.Characters.SELECTED, this, SignalNames.Characters.SELECTED_METHOD);
			gameStateManager.Connect(SignalNames.Characters.SELECTION_CLEARED, this, SignalNames.Characters.SELECTION_CLEARED_METHOD);
		}


		private void GetChildrenNodes()
		{
			faceIcon = GetNode<TextureRect>("CharacterIcon");
			healthbar = faceIcon.GetNode<HealthBar>("HealthBar");
			name = faceIcon.GetNode<Label>("Name");
		}


		private void _On_Character_Selected(Character character)
		{
			selectedCharacter = character;

			healthbar.SetValuesToCharacterValues(character);
			faceIcon.Texture = character.Stats.FaceIcon;
			name.Text = character.Stats.CharacterName;

			Show();
		}

		private void _On_Character_SelectionCleared()
		{
			GD.Print("Unselected");
			selectedCharacter = null;

			healthbar.CharacterSelectionCleared();

			Hide();
		}

		private void _On_Character_ReceivedDamage(Character character, int damage, Node origin, bool killingBlow)
		{
			if (character == null || character != selectedCharacter || killingBlow)
				return;

			healthbar.SetValueToCharactersCurrentHealth(character);
		}
	}
}
