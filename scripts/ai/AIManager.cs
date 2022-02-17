using System.Collections.Generic;
using System.Linq;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using Godot;

namespace FaffLatest.scripts.ai
{
    public class AIManager : Node
    {
        public static AIManager Instance;

        private HashSet<Character> aiCharacters => CharacterManager.Instance.AiCharacters;

        [Signal]
        public delegate void _Change_Turn(Faction faction);

        private bool isOurTurn;

        private Character currentlyActioningCharacter;
        private AiCharacterController characterController;

        private int currentArrayPos = 0;
        private bool haveMoreCharacters => aiCharacters != null && aiCharacters.Count > currentArrayPos;

        private AStarNavigator aStarNavigator;

        public override void _Ready()
        {
            aStarNavigator = AStarNavigator.Instance;
            Connect("_Change_Turn", GetNode(NodeReferences.Systems.GAMESTATE_MANAGER), "SetTurn");
            Instance = this;
            base._Ready();
        }

        private void ConnectCharacterSignals()
        {
            foreach (var character in aiCharacters)
            {
                character.ProperBody
                    .GetNode("AiCharacterController")
                    .Connect("_AiCharacter_TurnFinished", this, "_On__AiCharacter_TurnFinished");

                character.Connect(SignalNames.Characters.DISPOSING, this, SignalNames.Characters.DISPOSING_METHOD);
            }
        }


        public void SetAITurn(bool isTurn)
        {
            isOurTurn = isTurn;
            currentArrayPos = 0;
        }

        private void _On_Turn_Changed(string turn)
        {
            var ourTurn = turn.Equals("ENEMY");

            SetAITurn(ourTurn);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!isOurTurn)
                return;

            if (currentlyActioningCharacter == null && haveMoreCharacters)
            {
                GetNextCharacter();
            }
            else if(currentlyActioningCharacter == null && !haveMoreCharacters)
            {
                EndTurn();
            }
        }

        private void ClearCurrentlyActiveCharacter()
        {
            currentlyActioningCharacter.IsActive = false;
            currentlyActioningCharacter = null;
        }

        private void CantMoveCharacterFurther()
        {
            ClearCharacterForTurn();
            GetNextCharacter();
        }

        private void ClearCharacterForTurn()
        {
            while (currentlyActioningCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn > 0)
            {
                currentlyActioningCharacter.ProperBody.MovementStats.IncreaseAmountMovedThisTurn();
            }

            ClearCurrentlyActiveCharacter();
        }

        private void EndTurn()
        {

            currentArrayPos = 0;
            isOurTurn = false;

            EmitSignal("_Change_Turn", Faction.PLAYER);
            foreach (var character in aiCharacters)
            {
                character.ResetTurnStats();
            }

        }

        private void GetNextCharacter()
        {
            if (aiCharacters == null || currentArrayPos >= aiCharacters.Count)
            {
				CallDeferred("EndTurn");
                return;
            }

            currentlyActioningCharacter = aiCharacters.ElementAt(currentArrayPos);
            characterController = currentlyActioningCharacter.ProperBody.GetNode<AiCharacterController>("AiCharacterController");
            currentArrayPos++;

            if(currentlyActioningCharacter != null)
                characterController.SetOurTurn();
            else
                EndTurn();
        }

        private void _On_AiCharacter_TurnFinished(Character character)
        {
            currentlyActioningCharacter = null;
            characterController = null;
        }

        private void _On_Character_Disposing(Character character)
        {
            if (character is Character asChar)
            {
                if (IsCurrentlySelectedCharacter(character))
                {
                    GetNextCharacter();
                }

                aiCharacters.Remove(asChar);
            }
        }

        private bool IsCurrentlySelectedCharacter(Node character) => currentlyActioningCharacter != null && currentlyActioningCharacter == character;
    }
}
