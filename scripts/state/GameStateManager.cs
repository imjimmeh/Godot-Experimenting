using System;
using FaffLatest.scripts.ai;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.state
{
	public class GameStateManager : Node
	{
		private Character selectedCharacter;
		private Faction currentTurn;

		public Character SelectedCharacter => selectedCharacter;
		public Faction CurrentTurn => currentTurn;

		[Signal]
		public delegate void _Character_Selected(Character character);

		[Signal]
		public delegate void _Character_SelectionCleared();

		[Signal]
		public delegate void _Turn_Changed(string whoseTurn);

		public AStarNavigator AStarNavigator { get; private set; }
		public SpawnManager SpawnManager { get; private set; }

		public bool CharacterIsActive => SelectedCharacter != null && SelectedCharacter.IsActive;


		public override void _Ready()
		{
			base._Ready();

			AStarNavigator = GetNode<AStarNavigator>("../AStarNavigator");
			SpawnManager = GetNode<SpawnManager>("../SpawnManager");
			var aiManager = GetNode<AIManager>("../AIManager");
		   Connect("_Turn_Changed", GetNode("../UIManager"), "_On_Turn_Change");
			Connect("_Turn_Changed", aiManager, "_On_Turn_Change");

		}

		public void InitialiseMap()
		{
		}

		public void SetCurrentlySelectedCharacter(Character character)
		{
			selectedCharacter = character;
			EmitSignal(SignalNames.Characters.SELECTED, character);
		}

		public void ClearCurrentlySelectedCharacter()
		{
			selectedCharacter = null;
			EmitSignal(SignalNames.Characters.SELECTION_CLEARED);

			//GD.Print("Character has been unselected");
		}

		public void SetCurrentTurn(Faction turn)
		{
			currentTurn = turn;

			EmitSignal("_Turn_Changed", currentTurn.ToString());
		}

		private void _On_Character_FinishedMoving(Node character, Vector3 newPosition)
		{
			if (character is Character c)
			{
				SetCharacterActive(false);
				CheckTurnIsFinished(c);
			}
		}

		public void SetCharacterActive(bool isActive = true)
		{
			SetCharacterActive(SelectedCharacter, isActive);
		}

		public static void SetCharacterActive(Character character, bool isActive = true)
		{
			if(character == null)
				return;
				
			character.IsActive = isActive;
		}

		private void CheckTurnIsFinished(Character c)
		{
			//GD.Print($"Character {c.Stats.CharacterName} has finished a movement");

			var characterParentNode = c.Stats.IsPlayerCharacter ? GroupNames.PLAYER_CHARACTERS : GroupNames.AI_CHARACTERS;

			GD.Print($"{c.Stats.CharacterName} has fin their turn");
			GD.Print($"Checking for characters in {characterParentNode}");
			var charactersInGroup = GetTree().GetNodesInGroup(characterParentNode);

			GD.Print($"found {charactersInGroup.Count} chars in group");
			for (var x = 0; x < charactersInGroup.Count; x++)
			{
				var asCharacter = charactersInGroup[x] as Character;

				if (asCharacter.Stats.IsPlayerCharacter != c.Stats.IsPlayerCharacter)
					continue;

				if (asCharacter.Stats.CanMove)
				{
					GD.Print($"{asCharacter.Stats.CharacterName} can still move");
					//GD.Print($"Turn not over - {asCharacter.Stats.CharacterName} still has {asCharacter.Stats.AmountLeftToMoveThisTurn} movement left");
					return;
				}
			}

			GD.Print($"next turn");
			var nextTurn = c.Stats.IsPlayerCharacter ? Faction.ENEMY : Faction.PLAYER;
			SetCurrentTurn(nextTurn);
		}
	}
}
