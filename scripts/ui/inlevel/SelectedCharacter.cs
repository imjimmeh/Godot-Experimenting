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
        private const string ATTACK_ICON_NODE_NAME = "AttackIcon";
        private const string PORTRAIT_NODE_NAME = "CharacterIcon";
        private const string HEALTHBAR_NODE_NAME = "HealthBar";
        private const string MOVEMENT_ICON_NODE_NAME = "MovementIcon";
        private const string NAME_NODE_NAME = "Name";
        private TextureRect portrait;
		private HealthBar healthbar;
		private Label name;

		private IconWithText attackDisplay;
		private IconWithText movementDisplay;
		private Character selectedCharacter;


		public override void _Ready()
		{
			GetChildrenNodes();
			base._Ready();
		}

		public void ConnectSignals(Node gameStateManager)
		{
			gameStateManager.Connect(SignalNames.Characters.SELECTED, this, SignalNames.Characters.SELECTED_METHOD);
			gameStateManager.Connect(SignalNames.Characters.SELECTION_CLEARED, this, SignalNames.Characters.SELECTION_CLEARED_METHOD);
		}


		private void GetChildrenNodes()
		{
			portrait = GetNode<TextureRect>(PORTRAIT_NODE_NAME);


			attackDisplay = portrait.GetNode<IconWithText>(ATTACK_ICON_NODE_NAME);
			healthbar = portrait.GetNode<HealthBar>(HEALTHBAR_NODE_NAME);
			movementDisplay = portrait.GetNode<IconWithText>(MOVEMENT_ICON_NODE_NAME);
			name = portrait.GetNode<Label>(NAME_NODE_NAME);
		}


		private void _On_Character_Selected(Character character)
        {
            selectedCharacter = character;

            SetAttackDisplay(character);
            healthbar.SetValuesToCharacterValues(character);
            SetMovementDisplay(character);
            portrait.Texture = character.Stats.FaceIcon;
            name.Text = character.Stats.CharacterName;

            selectedCharacter.Connect(nameof(Character._Character_Attacking), this, "_On_Character_Attacking");
            CallDeferred("show");
        }

        private void SetMovementDisplay(Character character)
        {
            movementDisplay.SetLabelText(MovementIconText(character));
        }

        private void SetAttackDisplay(Character character)
        {
            attackDisplay.SetLabelText(AttackIconText(character));
        }

        private static string MovementIconText(Character character)
			=> $"{character.Body.MovementStats.AmountLeftToMoveThisTurn}/{character.Body.MovementStats.MaxMovementDistancePerTurn}";

		private static string AttackIconText(Character character)
			=> $"{character.Stats.EquippedWeapon.AttacksLeftThisTurn}/{character.Stats.EquippedWeapon.AttacksPerTurn}\n{character.Stats.EquippedWeapon.Name}";

        private void _On_Character_SelectionCleared()
		{
			if(selectedCharacter != null)
			{
				selectedCharacter.Disconnect(nameof(Character._Character_Attacking), this, "_On_Character_Attacking");
				selectedCharacter = null;
			}

			CallDeferred("hide");
		}

		private void _On_Character_ReceivedDamage(Character character, int damage, Node origin, bool killingBlow)
		{
			if (character == null || character != selectedCharacter || killingBlow)
				return;

			healthbar.SetValueToCharactersCurrentHealth(character);
		}

		private void _On_Character_Attacking(Character attacker, Character receiver)
		{
			if(attacker == selectedCharacter)
			{
				SetAttackDisplay(attacker);
			}
		}
	}
}
