using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.ai
{
	public class AIManager : Node
	{
		private Character[] aiCharacters;
		private bool isOurTurn;

		private Character currentlyActioningCharacter;

		private int currentArrayPos = 0;
		private bool haveMoreCharacters => aiCharacters != null && aiCharacters.Length > currentArrayPos;

		private AStarNavigator aStarNavigator;

		public void SetCharacters(Character[] characters)
		{
			GD.Print($"Received {characters?.Length} AI players");
			aiCharacters = characters;
		}

		public void SetAITurn(bool isTurn)
		{
			isOurTurn = isTurn;
			currentArrayPos = 0;
		}

		private void _On_Turn_Change(string turn)
		{
			var ourTurn = turn.Equals("ENEMY");
			GD.Print($"TUrn is {turn} our turn is {ourTurn}");

			SetAITurn(ourTurn);
		}

		public override void _Process(float delta)
		{
			base._Process(delta);

			if(isOurTurn && currentlyActioningCharacter != null)
			{
				MoveCharacterIfPossible();
			}
			else if(isOurTurn && currentlyActioningCharacter == null)
			{
				if (haveMoreCharacters)
				{
					GetNextCharacter();
				}
				else
				{
					ResetTurn();
				}
			}

		}

		private void MoveCharacterIfPossible()
		{
			if (currentlyActioningCharacter == null)
				return;

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
        }

        private void ClearCurrentlyActiveCharacter()
        {
            currentlyActioningCharacter.IsActive = false;
            currentlyActioningCharacter = null;
        }

        private void MoveCharacter()
		{
			var body = currentlyActioningCharacter.Body as MovingKinematicBody;

			var target = GetNearestPCToCharacter(body.Transform.origin);
			GD.Print($"{target.targetPosition}");
			var path = aStarNavigator.GetMovementPathNew(body.Transform.origin, target.targetPosition, currentlyActioningCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn);

			body.GetNode<PathMover>("PathMover").MoveWithPath(path);
			currentlyActioningCharacter.IsActive = true;
		}

		private void ResetTurn()
		{
			currentArrayPos = 0;
			isOurTurn = false;
		}

		private void GetNextCharacter()
		{
			currentlyActioningCharacter = aiCharacters[currentArrayPos];
			currentArrayPos++;

			GD.Print($"Getting next char");
		}

		public override void _Ready()
		{
			aStarNavigator = GetNode<AStarNavigator>(AStarNavigator.GLOBAL_SCENE_PATH);
			base._Ready();
		}

		public (Character closestChar, Vector3 targetPosition) GetNearestPCToCharacter(Vector3 ourCharPos)
		{
			Character closestCharacter = null;
			MovingKinematicBody body = null;

			Vector3 targetPos = Vector3.Zero;

			float closestDistance = 99999;

			foreach(var character in aStarNavigator.CharacterLocations)
			{
				var asChar = character.Key as Character;

				if (!asChar.Stats.IsPlayerCharacter)
					continue;

				body = asChar.Body as MovingKinematicBody;

				var vector = (body.Transform.origin - ourCharPos);
				var distance = vector.Length();
				if (distance < closestDistance)
				{
					var direction = vector.Normalized().Round();
					targetPos = body.Transform.origin - direction;
					closestDistance = distance;
					closestCharacter = asChar;
				}
			}

			return (closestCharacter, targetPos);
		}
	}	
}
