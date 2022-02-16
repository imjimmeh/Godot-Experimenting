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
		private TextureRect portrait;
		private HealthBar healthbar;
		private Label name;
		private Label movementText;

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
			portrait = GetNode<TextureRect>("CharacterIcon");
			healthbar = portrait.GetNode<HealthBar>("HealthBar");
			movementText = portrait.GetNode<Label>("MovementIcon/MovementText");
			name = portrait.GetNode<Label>("Name");
		}


		private void _On_Character_Selected(Character character)
		{
			selectedCharacter = character;

			healthbar.SetValuesToCharacterValues(character);
			portrait.Texture = character.Stats.FaceIcon;
			name.Text = character.Stats.CharacterName;
			movementText.Text = $"{character.ProperBody.MovementStats.AmountLeftToMoveThisTurn}/{character.ProperBody.MovementStats.MaxMovementDistancePerTurn}";
			
			CallDeferred("show");
		}

		private void _On_Character_SelectionCleared()
		{
			selectedCharacter = null;

			healthbar.CharacterSelectionCleared();

			CallDeferred("hide");
		}

		private void _On_Character_ReceivedDamage(Character character, int damage, Node origin, bool killingBlow)
		{
			if (character == null || character != selectedCharacter || killingBlow)
				return;

			healthbar.SetValueToCharactersCurrentHealth(character);
		}
	}
}
