using System;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.state
{
    public class GameStateManager : Node
    {
        private Character currentlySelectedCharacter;
        private CurrentTurn currentTurn;

        public Character CurrentlySelectedCharacter => currentlySelectedCharacter;
        public CurrentTurn CurrentTurn => currentTurn;

        [Signal]
        public delegate void _Character_Selected(Character character);

        [Signal]
        public delegate void _Character_SelectionCleared();

        [Signal]
        public delegate void _Turn_Changed(string whoseTurn);

        public AStarNavigator AStarNavigator { get; private set; }
        public SpawnManager SpawnManager { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            AStarNavigator = GetNode<AStarNavigator>("../AStarNavigator");
            SpawnManager = GetNode<SpawnManager>("../SpawnManager");

           Connect("_Turn_Changed", GetNode("../UIManager"), "_On_Turn_Change");
        }

        public void InitialiseMap()
        {
        }

        public void SetCurrentlySelectedCharacter(Character character)
        {
            currentlySelectedCharacter = character;
            EmitSignal(SignalNames.Characters.SELECTED, character);

            //GD.Print($"{character.Stats.CharacterName} has been selected");
        }

        public void ClearCurrentlySelectedCharacter()
        {
            currentlySelectedCharacter = null;
            EmitSignal(SignalNames.Characters.SELECTION_CLEARED);

            //GD.Print("Character has been unselected");
        }

        public void SetCurrentTurn(CurrentTurn turn)
        {
            currentTurn = turn;

            EmitSignal("_Turn_Changed", currentTurn.ToString());
        }

        private void _On_Character_TurnFinished(Node character)
        {
            //GD.Print($"Character has finished turn");
            var allPlayerCharacters = GetTree().GetNodesInGroup("playerCharacters");

            var triggeredCharacter = character as Character;
            for(var x = 0; x < allPlayerCharacters.Count; x++)
            {
                var asCharacter = allPlayerCharacters[x] as Character;

                if (asCharacter.Stats.IsPlayerCharacter != triggeredCharacter.Stats.IsPlayerCharacter)
                    continue;

                if(asCharacter.Stats.CanMove)
                    return;
            }

            var nextTurn = triggeredCharacter.Stats.IsPlayerCharacter ? CurrentTurn.ENEMY : CurrentTurn.PLAYER;

            SetCurrentTurn(nextTurn);
        }
    }
}