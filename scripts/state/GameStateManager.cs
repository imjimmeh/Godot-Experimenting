using System;
using System.Threading.Tasks;
using FaffLatest.scripts.ai;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.state
{
	public class GameStateManager : Node
	{
		private Character activeCharacter;

		private Character selectedCharacter;
		private Faction currentTurn;

		public Character ActiveCharacter => activeCharacter;
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

		public bool CharacterIsActive => ActiveCharacter != null && ActiveCharacter.IsActive;

		public bool HaveACharacterSelected => SelectedCharacter != null;

		public bool SelectedCharacterCanUseAction => HaveACharacterSelected && !SelectedCharacter.Stats.HasUsedActionThisTurn;

		public bool SelectedCharacterCanMove => HaveACharacterSelected && !SelectedCharacter.ProperBody.MovementStats.CanMove;

		public bool IsPlayerTurn => CurrentTurn == Faction.PLAYER;

		public override void _Ready()
		{
			base._Ready();

            AStarNavigator = GetNode<AStarNavigator>("../AStarNavigator");
            SpawnManager = GetNode<SpawnManager>("../SpawnManager");

            var aiManager = GetNode<AIManager>("../AIManager");

            Connect(SignalNames.State.TURN_CHANGE, GetNode("../UIManager"), SignalNames.State.TURN_CHANGE_METHOD);
            Connect(SignalNames.State.TURN_CHANGE, aiManager, SignalNames.State.TURN_CHANGE_METHOD);
        }

        public void InitialiseMap()
		{
		}

		public void SetCurrentlySelectedCharacter(Character character)
		{
			ClearCurrentlySelectedCharacter();

			selectedCharacter = character;
			EmitSignal(SignalNames.Characters.SELECTED, character);
		}

		public void ClearCurrentlySelectedCharacter()
		{
			selectedCharacter = null;
			EmitSignal(SignalNames.Characters.SELECTION_CLEARED);
		}

		public void PlayerEndTurn()
        {
            SetTurn(Faction.ENEMY);
        }

        private void ResetTurnStats(Godot.Collections.Array characters)
        {
            for (var x = 0; x < characters.Count; x++)
            {
                var asCharacter = characters[x] as Character;

                Task.Run(() => asCharacter.ResetTurnStats());
            }
        }


		private Godot.Collections.Array GetCurrentFactionCharacters() => GetFactionCharacters(CurrentTurn);

		private Godot.Collections.Array GetFactionCharacters(Faction faction)
		{
			var groupNameForCurrentCharacters = faction == Faction.PLAYER ? GroupNames.PLAYER_CHARACTERS : GroupNames.AI_CHARACTERS;
			var characters = GetTree().GetNodesInGroup(groupNameForCurrentCharacters);

			return characters;
		}


		public void SetTurn(Faction nextTurn)
		{
			if (currentTurn == nextTurn)
				return;

			var nextFactionCharacters = GetFactionCharacters(nextTurn);
			ResetTurnStats(nextFactionCharacters);

			ClearCurrentlySelectedCharacter();

			currentTurn = nextTurn;

			EmitSignal(SignalNames.State.TURN_CHANGE, currentTurn.ToString());
		}

		private void _On_Character_FinishedMoving(Character character, Vector3 newPosition)
        {
            SetCharacterActive(character, false);
			CheckTurnFinishedAndProcess();
        }

		public void SetCharacterActive(Character character, bool isActive = true)
		{
			GD.Print($"Setting character active");
			activeCharacter = character;

			if(character == null)
            {
				GD.Print($"But character is null?");
				return;
            }

			character.IsActive = isActive;
		}

		public void CheckTurnFinishedAndProcess()
        {
			var characters = GetCurrentFactionCharacters();

			if (!HasTurnFinished(characters))
            {
				return;
            }

			var nextTurn = GetNextTurn(CurrentTurn);
			SetTurn(nextTurn);
		}

		private bool HasTurnFinished(Godot.Collections.Array characters = null)
        {
			if(characters == null)
				characters = GetCurrentFactionCharacters();

            for (var x = 0; x < characters.Count; x++)
            {
                var asCharacter = characters[x] as Character;

                if (asCharacter.ProperBody.MovementStats.CanMove)
                {
					return false;
                }
            }

			return true;
        }

		private static Faction GetNextTurn(Faction faction)
        {
            switch (faction)
            {
				case Faction.PLAYER:
					return Faction.ENEMY;
				case Faction.ENEMY:
					return Faction.PLAYER;
			}

			return Faction.PLAYER;
        }

		private void _On_Character_Disposing(Character character)
        {
			if (this.ActiveCharacter == character)
				activeCharacter = null;

			if (this.SelectedCharacter == character)
				selectedCharacter = null;
        }
    }
}
