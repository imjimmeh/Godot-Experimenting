using FaffLatest.scripts.characters;
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
			GD.Print("Press");
			if (mouseButtonEvent.ButtonIndex == 1 && gameStateManager.SelectedCharacter != null && !gameStateManager.SelectedCharacter.Stats.HasUsedActionThisTurn && !character.Stats.IsPlayerCharacter )
			{
				//GD.Print("yes");
				GD.Print("$attack");
				character._On_Character_ReceiveDamage(5, character);

			}
		}

		private void _On_Character_Mouse_Released(Character character, InputEventMouseButton mouseButtonEvent)
		{
			if(mouseButtonEvent.ButtonIndex == 1 && character.Stats.IsPlayerCharacter && !gameStateManager.CharacterIsActive)
			{
				//GD.Print("yes");
				gameStateManager.SetCurrentlySelectedCharacter(character);
			}
			else
			{
				//GD.Print($"Unhandled character mouse released event Mb.i is {mouseButtonEvent.ButtonIndex} - playercharacter is {character.Stats.IsPlayerCharacter}");
			}
		}

		private void _On_World_ClickedOn(ClickableWorldElement world, InputEventMouseButton mouseButtonEvent, Vector3 position)
		{
			//GD.Print($"Mouse button {mouseButtonEvent.ButtonIndex} was {(mouseButtonEvent.Pressed ? "Pressed" : "Released")} on world");

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
			if (mouseButtonEvent.ButtonIndex == 1 && gameStateManager.SelectedCharacter != null)
			{
				gameStateManager.ClearCurrentlySelectedCharacter();
			}
			else if (mouseButtonEvent.ButtonIndex == 2 && gameStateManager.SelectedCharacter != null)
			{
				if (gameStateManager.SelectedCharacter.Stats.CanMove)
				{
					//IssueMoveOrder(position);
				}
			}
			else
			{ 
				//GD.Print($"Unhandled world mouse released event Mb.i is {mouseButtonEvent.ButtonIndex}");
			}

		}

		private void _On_Character_MoveOrder(Vector3 position)
		{
			var canMove = gameStateManager?.SelectedCharacter?.Stats?.CanMove;
			if (canMove.HasValue && canMove.Value)
			{
				IssueMoveOrder(position);
			}
		}

		private void IssueMoveOrder(Vector3 position)
		{
			position = position.Round();

			(var x, var y, var z) = (position.x, 1, position.z);
			x = Mathf.Clamp(x, 1, 50);
			z = Mathf.Clamp(z, 1, 50);

			position = new Vector3(x, 1, z);

			var body = gameStateManager.SelectedCharacter.GetNode<KinematicBody>("KinematicBody");

			var distance = (position - body.Transform.origin).Length();

			if(distance > gameStateManager.SelectedCharacter.Stats.MaxMovementDistancePerTurn)
			{
				//GD.Print($"Distance is {distance} - original position is {position}");
				position = body.Transform.origin.MoveToward(position, gameStateManager.SelectedCharacter.Stats.MaxMovementDistancePerTurn);

				position = position.Round();
				//GD.Print($"New position is {position}");
			}

			var convertedPath = aStarNavigator.GetMovementPath(body.Transform.origin, position, gameStateManager.SelectedCharacter.Stats.MaxMovementDistancePerTurn); // navigation.GetMovementPathNodes(body.Transform, position);

			if(convertedPath == null)
			{
				//GD.Print("Cannot move here");
				return;
			}

			//GD.Print($"We can move {gameStateManager.SelectedCharacter}");
			if(convertedPath.Length > gameStateManager.SelectedCharacter.Stats.MaxMovementDistancePerTurn)
			{
				Array.Resize(ref convertedPath, gameStateManager.SelectedCharacter.Stats.MaxMovementDistancePerTurn);
			}

			EmitSignal(SignalNames.Characters.MOVE_TO, gameStateManager.SelectedCharacter, convertedPath);
			gameStateManager.SetCharacterActive(true);
		}
	}
}
