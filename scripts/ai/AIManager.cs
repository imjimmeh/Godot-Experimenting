using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using Godot;

namespace FaffLatest.scripts.ai
{
	public class AIManager : Node
	{
		[Signal]
		public delegate void _Change_Turn(Faction faction);

		private List<Character> aiCharacters;
		private bool isOurTurn;

		private Character currentlyActioningCharacter;

		private int currentArrayPos = 0;
		private bool haveMoreCharacters => aiCharacters != null && aiCharacters.Count > currentArrayPos;

		private AStarNavigator aStarNavigator;

		public void SetCharacters(Character[] characters)
		{
			aiCharacters = characters.ToList();

			ConnectCharacterSignals();
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

			if (currentlyActioningCharacter != null)
			{
				MoveCharacterIfPossible();
			}
			else if (haveMoreCharacters)
			{
				GetNextCharacter();
			}
			else
			{
				ResetTurn();
			}
		}

		private void MoveCharacterIfPossible()
		{
			bool inactiveCurrentCharacterWithMovementLeft = !currentlyActioningCharacter.IsActive && currentlyActioningCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn > 0;
			if (inactiveCurrentCharacterWithMovementLeft)
			{
				GD.Print("Moving AI");
				MoveCharacter();
			}
			else if (currentlyActioningCharacter.IsActive && currentlyActioningCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn <= 0)
			{
				GD.Print($"SActive and etting AI char done");
				ClearCurrentlyActiveCharacter();
			}
			else
			{
				GD.Print($"{currentlyActioningCharacter}, {currentlyActioningCharacter.IsActive}, {currentlyActioningCharacter.ProperBody.MovementStats}");
				if (!currentlyActioningCharacter.ProperBody.HaveDestination)
				{
					ClearCharacterForTurn();

					GD.Print($"HERE");
					if (haveMoreCharacters)
						GetNextCharacter();
					else
						ResetTurn();
				}
			}
		}

		private void ClearCurrentlyActiveCharacter()
		{
			currentlyActioningCharacter.IsActive = false;
			currentlyActioningCharacter = null;
		}

		private void MoveCharacter()
		{
			var (_, targetPosition) = GetNearestPCToCharacter(currentlyActioningCharacter.ProperBody.Transform.origin);

			var vector = (currentlyActioningCharacter.ProperBody.Transform.origin - targetPosition).Abs();
			var distance = vector.x + vector.z;

            if (distance < 1.00001f)
            {
				GD.Print($"Next to player - clearing");
				ClearCharacterForTurn();
				return;
            }

			var foundEmptyPosition = aStarNavigator.TryGetNearestEmptyPointToLocationWithLoop(targetPosition, currentlyActioningCharacter.ProperBody.Transform.origin, out Vector3 foundPoint, 5);

			if(!foundEmptyPosition)
            {
				CantMoveCharacterFurther(targetPosition);
				return;
            }

			GD.Print($"Found {foundPoint} for {targetPosition}");

            var path = aStarNavigator.GetMovementPath(
				start: currentlyActioningCharacter.ProperBody.Transform.origin, 
				end:foundPoint, 
				character: currentlyActioningCharacter);

			if(path == null)
            {
                CantMoveCharacterFurther(targetPosition);
                return;
            }

            GD.Print($"Moving with path {string.Join(",", path)}");
			currentlyActioningCharacter.ProperBody.GetNode<PathMover>("PathMover").MoveWithPath(path);
			currentlyActioningCharacter.IsActive = true;
		}

        private void CantMoveCharacterFurther(Vector3 targetPosition)
        {
            GD.Print($" could not find any path for {targetPosition} - we are {targetPosition - currentlyActioningCharacter.ProperBody.Transform.origin} far away");
            ClearCharacterForTurn();

            if (haveMoreCharacters)
                GetNextCharacter();
        }

        private void ClearCharacterForTurn()
        {
            while (currentlyActioningCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn > 0)
            {
                currentlyActioningCharacter.ProperBody.MovementStats.IncreaseAmountMovedThisTurn();
                GD.Print($"Decreased movement to {currentlyActioningCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn}");
            }

			ClearCurrentlyActiveCharacter();

			GD.Print($"OUT");
		}

        private void ResetTurn()
		{
			GD.Print($"Ressetting turn");

			foreach(var character in aiCharacters)
            {
				character.ResetTurnStats();
            }
				
			currentArrayPos = 0;
			isOurTurn = false;

			EmitSignal("_Change_Turn", Faction.PLAYER);
		}

		private void GetNextCharacter()
		{
			GD.Print($"Getting next char");

			if(currentArrayPos > aiCharacters.Count)
			{
				ClearCurrentlyActiveCharacter();
				currentArrayPos = 0;
				return;
			}

			currentlyActioningCharacter = aiCharacters[currentArrayPos];
			currentArrayPos++;

			if (currentlyActioningCharacter == null)
			{
				GD.Print($"Character null - processing next if possible");
				if (haveMoreCharacters)
					GetNextCharacter();
				else
					ClearCurrentlyActiveCharacter();
			}

		}

		public override void _Ready()
		{
			aStarNavigator = GetNode<AStarNavigator>(NodeReferences.Systems.ASTAR);
			Connect("_Change_Turn", GetNode(NodeReferences.Systems.GAMESTATE_MANAGER), "SetTurn");
			base._Ready();
		}

		public (Character closestChar, Vector3 targetPosition) GetNearestPCToCharacter(Vector3 ourCharPos)
		{
			Character closestCharacter = null;
			Vector3 targetPos = Vector3.Zero;

			float closestDistance = 99999;

			foreach(var character in aStarNavigator.CharacterLocations)
			{
				var asChar = character.Key as Character;

				if (!asChar.Stats.IsPlayerCharacter)
					continue;

				var body = asChar.Body as MovingKinematicBody;

				var vector = (body.Transform.origin - ourCharPos);
				var distance = vector.Length();
				if (distance < closestDistance)
				{
					var direction = vector.Normalized().Round();
					targetPos = body.Transform.origin - direction;
					closestDistance = distance;
					closestCharacter = asChar;

					GD.Print($"{asChar.Stats.CharacterName} is new closest player at -{body.Transform.origin} - our target pos is {targetPos}, distance is {closestDistance}, direction {direction}, vector {vector}") ;
				}
			}

			return (closestCharacter, targetPos);
		}

		private void ConnectCharacterSignals()
		{
			foreach(var character in aiCharacters)
			{
				character.ProperBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, this, "_On_AICharacter_FinishedMoving");
				character.Connect(SignalNames.Characters.DISPOSING, this, SignalNames.Characters.DISPOSING_METHOD);
			}
		}

		private void _On_AICharacter_FinishedMoving(Node character, Vector3 newPosition)
		{
			GD.Print($"Char finished moving");
			if (haveMoreCharacters)
				GetNextCharacter();
			else
				ClearCurrentlyActiveCharacter();
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
