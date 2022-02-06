using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.state;
using FaffLatest.scripts.world;
using Godot;
using System;

namespace FaffLatest.scripts.input
{
	public class InputManager : Node
	{
		private GameStateManager gameStateManager;

		private Navigation navigation;

		[Signal]
		public delegate void _Character_MoveTo(Character character, Vector3[] target);

		public override void _Ready()
		{
			base._Ready();

			gameStateManager = GetNode<GameStateManager>("../GameStateManager");
			navigation = GetNode<Navigation>("/root/Root/Environment/Navigation");
		}

		public void ConnectWorldClickedOnSignal(Node element)
		{
			element.Connect(constants.SignalNames.World.CLICKED_ON, this, "_On_World_ClickedOn");
		}

		private void _On_Character_ClickedOn(Character character, InputEventMouseButton mouseButtonEvent)
		{
			GD.Print($"Mouse button {mouseButtonEvent.ButtonIndex} was {(mouseButtonEvent.Pressed ? "Pressed" : "Released")} on character {character.Stats.CharacterName}");

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

		}

		private void _On_Character_Mouse_Released(Character character, InputEventMouseButton mouseButtonEvent)
		{
			if(mouseButtonEvent.ButtonIndex == 1 && character.Stats.IsPlayerCharacter)
			{
				GD.Print("yes");
				gameStateManager.SetCurrentlySelectedCharacter(character);
			}
			else
			{
				GD.Print($"Unhandled character mouse released event Mb.i is {mouseButtonEvent.ButtonIndex} - playercharacter is {character.Stats.IsPlayerCharacter}");
			}
		}

		private void _On_World_ClickedOn(ClickableWorldElement world, InputEventMouseButton mouseButtonEvent, Vector3 position)
		{
			GD.Print($"Mouse button {mouseButtonEvent.ButtonIndex} was {(mouseButtonEvent.Pressed ? "Pressed" : "Released")} on world");

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
			if (mouseButtonEvent.ButtonIndex == 1 && gameStateManager.CurrentlySelectedCharacter != null)
			{
				gameStateManager.ClearCurrentlySelectedCharacter();
			}
			else if (mouseButtonEvent.ButtonIndex == 2 && gameStateManager.CurrentlySelectedCharacter != null)
			{
				IssueMoveOrder(position);
			}
			else
			{ 
				GD.Print($"Unhandled world mouse released event Mb.i is {mouseButtonEvent.ButtonIndex}");
			}

		}

		private void IssueMoveOrder(Vector3 position)
		{
			var body = gameStateManager.CurrentlySelectedCharacter.GetNode<KinematicBody>("KinematicBody");
			var path = navigation.GetSimplePath(body.Transform.origin, position);
			EmitSignal(SignalNames.Characters.MOVE_TO, gameStateManager.CurrentlySelectedCharacter, path);

			gameStateManager.ClearCurrentlySelectedCharacter();
		}
	}
}
