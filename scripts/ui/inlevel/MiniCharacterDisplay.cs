using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.state;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.ui
{
	public class MiniCharacterDisplay : Control
	{
		[Signal]
		public delegate void _Portrait_Clicked(Node character);

		[Export]
		public DynamicFont FontToUse { get; set; }

		[Export]
		public int FontSize { get; set; } = 32;

		[Export]
		public float MaxWidthOfIcon { get; set; } = 32;

		private TextureRect characterPortrait;
		private Label characterName;
		private Character character;
		private IconWithText movementIcon;
		private IconWithText attackIcon;

		private bool mouseIsOver = false;

		public MiniCharacterDisplay(Character character)
		{
			this.character = character;
		}

		public MiniCharacterDisplay()
		{
			
		}

		public override void _Ready()
		{
			base._Ready();
			GetChildrenNodes();

			InitialiseFont();

			characterName.AddFontOverride("font", FontToUse);

			
			Connect("_Portrait_Clicked", GetNode(NodeReferences.Systems.INPUT_MANAGER), "_On_Character_PortraitClicked");
			Connect("mouse_entered", this, "_MouseEntered");
			Connect("mouse_exited", this, "_MouseExited");

			GetNode(NodeReferences.Systems.GAMESTATE_MANAGER).Connect(SignalNames.State.TURN_CHANGE, this, SignalNames.State.TURN_CHANGE_METHOD);
		}

		private void InitialiseFont()
		{
			FontToUse.Size = FontSize;
			FontToUse.OutlineSize = 1;
			FontToUse.OutlineColor = new Color(0, 0, 0, 1);
			FontToUse.UseFilter = true;
		}

		private void GetChildrenNodes()
		{
			attackIcon = GetNode<IconWithText>("AttackIcon");
			characterPortrait = GetNode<TextureRect>("Portrait");
			characterName = GetNode<Label>("CharacterName");
			movementIcon = GetNode<IconWithText>("MovementIcon");
		}

		public void SetCharacter(Character character)
		{
			this.character = character;
			this.character.Body.Connect(SignalNames.Characters.MOVEMENT_FINISHED, this, SignalNames.Characters.MOVEMENT_FINISHED_METHOD);
			this.character.Connect(nameof(Character._Character_ReceivedDamage), this, $"_On{nameof(Character._Character_ReceivedDamage)}");
			this.character.Connect(nameof(Character._Character_Attacking), this, $"_On{nameof(Character._Character_Attacking)}");
		}


		public override void _Process(float delta)
		{
			base._Process(delta);

			if (TextureNotInitialisedButCharacterIs)
			{
				SetValues();
			}
		}

		private void SetValues()
		{
			characterPortrait.Texture = character.Stats.FaceIcon;

			if (TextureIsTooLarge())
			{
				ResizeTexture();
			}

			characterName.Text = character.Stats.CharacterName;
		}

		private bool TextureNotInitialisedButCharacterIs => characterPortrait.Texture == null && character != null && character?.Stats != null && character.Stats.FaceIcon != null;

		private void ResizeTexture()
		{
			var div = MaxWidthOfIcon / characterPortrait.Texture.GetWidth();
			RectScale = new Vector2(div, div);
		}

		private bool TextureIsTooLarge() => characterPortrait.Texture.GetWidth() > MaxWidthOfIcon;

		public override void _Input(InputEvent inputEvent)
		{
			if(mouseIsOver && inputEvent is InputEventMouseButton mouseButton)
			{
				if(mouseButton.ButtonIndex == 1 && !mouseButton.Pressed)
				{
					EmitSignal("_Portrait_Clicked", character);
				}
			}

			base._Input(inputEvent);
		}

		private void _On_Character_FinishedMoving(Character character, Vector3 newPosition)
		{
			if (!character.Body.MovementStats.CanMove)
            {
                HideMovementIcon();
            }

        }

        private void HideMovementIcon()
        {
            movementIcon.CallDeferred("hide");
        }

        private void _On_Turn_Changed(string whoseTurn)
		{
			if(character.Body.MovementStats.CanMove)
			{
				movementIcon.CallDeferred("show");
				attackIcon.CallDeferred("show");
			}
		}

		private void _MouseEntered()
		{
			mouseIsOver = true;
		}

		private void _MouseExited()
		{
			mouseIsOver = false;
		}

		private void _On_Character_ReceivedDamage(Character character, int damage, Vector3 origin, bool killingBlow)
		{
			if(killingBlow)
			{
				CallDeferred("queue_free");
			}
		}

		
        private void _On_Character_Attacking(Character attacker, Character receiver)
        {
            if(attacker.Stats.EquippedWeapon.AttacksLeftThisTurn == 0)
            {
				HideMovementIcon();
                HideAttackIcon();
            }
        }

        private void HideAttackIcon()
        {
            attackIcon.CallDeferred("hide");
        }
    }
}
