using FaffLatest.scripts.attacks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.combat;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.shared;
using FaffLatest.scripts.state;
using FaffLatest.scripts.ui;
using FaffLatest.scripts.world;
using Godot;
using System;
using System.Threading.Tasks;

namespace FaffLatest.scripts.input
{
	public class InputManager : Node
	{
		private GameStateManager gameStateManager;
		private AStarNavigator aStarNavigator;

		[Signal]
		public delegate void _Character_MoveTo(Character character, Vector3[] target);

		[Signal]
		public delegate void _Camera_MoveToPosition(Vector3 position);

		public override void _Ready()
		{
			base._Ready();

			aStarNavigator = GetNode<AStarNavigator>("../AStarNavigator");
			gameStateManager = GetNode<GameStateManager>("../GameStateManager");
		}

		private void _On_Character_ClickedOn(Character character, InputEventMouseButton mouseButtonEvent)
		{
			if(mouseButtonEvent.Pressed)	
			{
				_On_Character_Mouse_Pressed(character, mouseButtonEvent);
			}
			else if (!mouseButtonEvent.Pressed)
			{
				_On_Character_Mouse_Released(character, mouseButtonEvent);
			}
		}

		private void _On_Character_PortraitClicked(Character character)
        {
            ProcessCharacterClick(character);
        }

        private void ProcessCharacterClick(Character character)
        {
			if (!gameStateManager.IsPlayerTurn || gameStateManager.CharacterIsActive)
                return;

            if (gameStateManager.HaveACharacterSelected && character == gameStateManager.SelectedCharacter)
            {
				EmitSignal(SignalNames.Cameras.MOVE_TO_POSITION, gameStateManager.SelectedCharacter.Body.GlobalTransform.origin);
            }
            else
            {
				gameStateManager.SetCurrentlySelectedCharacter(character);
            }
        }

        private void _On_Character_Mouse_Pressed(Character character, InputEventMouseButton mouseButtonEvent)
        {

        }

        private async void _On_Character_Mouse_Released(Character character, InputEventMouseButton mouseButtonEvent)
		{
            if (mouseButtonEvent.IsSelectCharacterAction(gameStateManager, character))
			{
				gameStateManager
					.SetCurrentlySelectedCharacter(character);
			}
			else if(mouseButtonEvent.IsAttackCommand(gameStateManager, character))
            {
				await AttackHelpers.TryAttack(gameStateManager.SelectedCharacter, character);
            }
        }

        private static async Task ConfirmMovementLeft(Character character)
        {
            var proceed = await UIManager.Instance.ConfirmCharacterAttack(character);

            if (!proceed)
            {
                UIManager.Instance.SpawnDamageLabel(character.Body.GlobalTransform.origin, "Cancelling!");
            }
        }

        private static bool ProceedWithAttack(Character character, weapons.AttackResult canAttackTarget)
        {
            switch (canAttackTarget)
            {
                case weapons.AttackResult.OutOfRange:
                    {
                        UIManager.Instance.SpawnDamageLabel(character.Body.GlobalTransform.origin, "Out of range!");
                        return false;
                    }

                case weapons.AttackResult.OutOfAttacksForTurn:
                    {
                        UIManager.Instance.SpawnDamageLabel(character.Body.GlobalTransform.origin, "Out of attacks!");
                        return false;
                    }
            }

			return true;
        }

        private void _On_World_ClickedOn(ClickableWorldElement world, InputEventMouseButton mouseButtonEvent, Vector3 position)
		{
			if (mouseButtonEvent.Pressed)
			{
				_On_World_Mouse_Pressed(world, mouseButtonEvent);
			}
			else if (!mouseButtonEvent.Pressed)
			{
				_On_World_Mouse_Released(world, mouseButtonEvent, position);
			}
		}

		private void _On_World_Mouse_Pressed(ClickableWorldElement world, InputEventMouseButton mouseButtonEvent)
		{

		}

		private void _On_World_Mouse_Released(ClickableWorldElement world, InputEventMouseButton mouseButtonEvent, Vector3 position)
		{
			bool isClearSelectionAction = mouseButtonEvent.IsLMB() && gameStateManager.HaveACharacterSelected;

            if (isClearSelectionAction)
			{
				gameStateManager.ClearCurrentlySelectedCharacter();
			}
			else if (mouseButtonEvent.ButtonIndex == 2 && gameStateManager.SelectedCharacter != null)
			{
			}
			else
			{ 
			}

		}

		private void _On_Character_MoveOrder(Vector3 position)
		{
			var canMove = gameStateManager?.SelectedCharacter?.Body.MovementStats.CanMove;

			if (canMove.HasValue && canMove.Value)
			{
				IssueMoveOrder(position);
			}
		}

		private void IssueMoveOrder(Vector3 position)
		{		
            position = position.Round();
            position = position.CopyYValue(gameStateManager.SelectedCharacter.Body.Transform.origin);
            position = GetTargetPositionClampedByMovementDistance(position, gameStateManager.SelectedCharacter.Body);

            var result = aStarNavigator.TryGetMovementPath(
				start: gameStateManager.SelectedCharacter.Body.Transform.origin, 
				end: position, 
				character: gameStateManager.SelectedCharacter); 

            if (result == null && result.CanFindPath && result.Path?.Length > 0)
                return;

			gameStateManager.SetCharacterActive(gameStateManager.SelectedCharacter, true);
			EmitSignal(SignalNames.Characters.MOVE_TO, gameStateManager.SelectedCharacter, result.Path);
			gameStateManager.ClearCurrentlySelectedCharacter();
		}

		private Vector3 GetTargetPositionClampedByMovementDistance(Vector3 position, KinematicBody body)
        {
            var distance = (position - body.Transform.origin).Length();

            if (distance > gameStateManager.SelectedCharacter.Body.MovementStats.AmountLeftToMoveThisTurn)
            {
				position = body.Transform.origin.MoveToward(position, gameStateManager.SelectedCharacter.Body.MovementStats.AmountLeftToMoveThisTurn);
                position = position.Round();
            }

            return position;
        }
    }
}
