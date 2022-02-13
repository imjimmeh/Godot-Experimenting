using FaffLatest.scripts.characters;
using FaffLatest.scripts.combat;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using FaffLatest.scripts.world;
using Godot;
using System;

namespace FaffLatest.scripts.input
{
	public class InputManager : Node
	{
		private GameStateManager gameStateManager;
		private AStarNavigator aStarNavigator;

		[Signal]
		public delegate void _Character_MoveTo(Character character, Vector3[] target);

		public override void _Ready()
		{
			base._Ready();

			aStarNavigator = GetNode<AStarNavigator>("../AStarNavigator");
			gameStateManager = GetNode<GameStateManager>("../GameStateManager");
		}

		private void _On_Character_ClickedOn(Character character, InputEventMouseButton mouseButtonEvent)
		{
			//GD.Print($"Mouse button {mouseButtonEvent.ButtonIndex} was {(mouseButtonEvent.Pressed ? "Pressed" : "Released")} on character {character.Stats.CharacterName}");

			if(mouseButtonEvent.Pressed)	
			{
				_On_Character_Mouse_Pressed(character, mouseButtonEvent);
			}
			else if (!mouseButtonEvent.Pressed)
			{
				_On_Character_Mouse_Released(character, mouseButtonEvent);
			}
		}

		private void _On_Character_Mouse_Pressed(Character character, InputEventMouseButton mouseButtonEvent)
        {
            if (IsAttackCommand(character, mouseButtonEvent))
            {
				gameStateManager.SelectedCharacter.TryIssueAttackCommand(character);
			}
            else
            {

            }
        }

        private bool IsAttackCommand(Character character, InputEventMouseButton mouseButtonEvent)
			=> mouseButtonEvent.ButtonIndex == 1 && gameStateManager.SelectedCharacter != null && !gameStateManager.SelectedCharacter.Stats.HasUsedActionThisTurn && !character.Stats.IsPlayerCharacter;

        private void _On_Character_Mouse_Released(Character character, InputEventMouseButton mouseButtonEvent)
		{
            bool isSelectChararacterAction = mouseButtonEvent.ButtonIndex == 1 && character.Stats.IsPlayerCharacter && !gameStateManager.CharacterIsActive;

            if (isSelectChararacterAction)
			{
				gameStateManager.SetCurrentlySelectedCharacter(character);
			}
			else
			{
			}
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
            bool isClearSelectionAction = mouseButtonEvent.ButtonIndex == 1 && gameStateManager.SelectedCharacter != null;

            if (isClearSelectionAction)
			{
				gameStateManager.ClearCurrentlySelectedCharacter();
			}
			else if (mouseButtonEvent.ButtonIndex == 2 && gameStateManager.SelectedCharacter != null)
			{
			}
			else
			{ 
				//GD.Print($"Unhandled world mouse released event Mb.i is {mouseButtonEvent.ButtonIndex}");
			}

		}

		private void _On_Character_MoveOrder(Vector3 position)
		{
			var canMove = gameStateManager?.SelectedCharacter?.ProperBody.MovementStats.CanMove;

			if (canMove.HasValue && canMove.Value)
			{
				IssueMoveOrder(position);
			}
		}

		private void IssueMoveOrder(Vector3 position)
        {
            var body = gameStateManager.SelectedCharacter.GetNode<KinematicBody>("KinematicBody");
            position = position.Round();

            (var x, var y, var z) = (position.x, body.Transform.origin.y, position.z);

            x = Mathf.Clamp(x, 1, 50);
            z = Mathf.Clamp(z, 1, 50);

            position = new Vector3(x, y, z);

            position = GetTargetPositionClampedByMovementDistance(position, body);

            GD.Print($"Getting path from {body.Transform.origin} to {position}");

            var convertedPath = aStarNavigator.GetMovementPath(body.Transform.origin, position, gameStateManager.SelectedCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn); // navigation.GetMovementPathNodes(body.Transform, position);

            if (convertedPath == null)
            {
                GD.Print($"Cannot move from {body.Transform.origin} to {position}");
                return;
            }

			EmitSignal(SignalNames.Characters.MOVE_TO, gameStateManager.SelectedCharacter, convertedPath);
            gameStateManager.SetCharacterActive(true);
        }

        private Vector3 GetTargetPositionClampedByMovementDistance(Vector3 position, KinematicBody body)
        {
            var distance = (position - body.Transform.origin).Length();

            if (distance > gameStateManager.SelectedCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn)
            {
				position = body.Transform.origin.MoveToward(position, gameStateManager.SelectedCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn);
                position = position.Round();
            }

            return position;
        }
    }
}
