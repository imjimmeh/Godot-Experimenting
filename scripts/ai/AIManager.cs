using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.ai
{
	public class AIManager : Node
	{
		private List<Character> aiCharacters;
		private bool isOurTurn;

		private Character currentlyActioningCharacter;

		private int currentArrayPos = 0;
		private bool haveMoreCharacters => aiCharacters != null && aiCharacters.Count > currentArrayPos;

		private AStarNavigator aStarNavigator;

		public void SetCharacters(Character[] characters)
		{
			GD.Print($"Received {characters?.Length} AI players");
			aiCharacters = characters.ToList();

			ConnectCharacterSignals();
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
				//GD.Print("moving");
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
			else
			{

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
				//GD.Print($"{currentlyActioningCharacter.ProperBody.Transform.origin} - {currentlyActioningCharacter?.IsActive} - {currentlyActioningCharacter?.ProperBody?.MovementStats}");
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
			var path = aStarNavigator.GetMovementPath(body.Transform.origin, target.targetPosition, currentlyActioningCharacter.ProperBody.MovementStats.AmountLeftToMoveThisTurn);

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

		private void ConnectCharacterSignals()
		{
			foreach(var character in aiCharacters)
			{
				var asCharacter = character as Character;

				asCharacter.ProperBody.Connect("_Character_FinishedMoving", this, "_On_AICharacter_FinishedMoving");
				asCharacter.Connect(SignalNames.Characters.DISPOSING, this, SignalNames.Characters.DISPOSING_METHOD);
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

		private void _On_Character_Disposing(Node character)
        {
			if (character.Equals(currentlyActioningCharacter) && character is Character asChar)
            {
				aiCharacters.Remove(asChar);
				GetNextCharacter();
            }
        }
	}	
}
