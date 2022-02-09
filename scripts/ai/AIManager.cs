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
			if (!currentlyActioningCharacter.IsActive && currentlyActioningCharacter.Stats.AmountLeftToMoveThisTurn > 0)
            {
                MoveCharacter();
            }
            else if (currentlyActioningCharacter.IsActive && currentlyActioningCharacter.Stats.AmountLeftToMoveThisTurn <= 0)
			{
				currentlyActioningCharacter.IsActive = false;
				currentlyActioningCharacter = null;
			}
			else if(currentlyActioningCharacter.Stats.AmountLeftToMoveThisTurn <= 0)
            {
				currentlyActioningCharacter = null;
			}
		}

        private void MoveCharacter()
        {
			var body = currentlyActioningCharacter.Body as CharacterKinematicBody;

			var target = GetNearestPCToCharacter(body.Transform.origin);
			GD.Print($"{target.targetPosition}");
            var path = aStarNavigator.GetMovementPath(body.Transform.origin, target.targetPosition, currentlyActioningCharacter.Stats.AmountLeftToMoveThisTurn);

            body.MoveWithPath(path);
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
		}

		public override void _Ready()
		{
			aStarNavigator = GetNode<AStarNavigator>(AStarNavigator.GLOBAL_SCENE_PATH);
			base._Ready();
		}

		public (Character closestChar, Vector3 targetPosition) GetNearestPCToCharacter(Vector3 ourCharPos)
        {
			Character closestCharacter = null;
			CharacterKinematicBody body = null;

			Vector3 targetPos = Vector3.Zero;

			float closestDistance = 99999;

			foreach(var character in aStarNavigator.CharacterLocations)
            {
				var asChar = character.Key as Character;

				if (!asChar.Stats.IsPlayerCharacter)
					continue;

				body = asChar.Body as CharacterKinematicBody;

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
