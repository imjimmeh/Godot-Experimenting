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
		private bool haveMoreCharacters => aiCharacters != null && aiCharacters.Length > currentArrayPos + 1;

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
				GD.Print($"trying to mvoe char");
				MoveCharacterIfPossible();
			}
			else if(isOurTurn && currentlyActioningCharacter == null)
			{
				GD.Print($"Our turn, no char selected");
				if (haveMoreCharacters)
				{
					GD.Print($"getting next char");
					GetNextCharacter();
				}
				else
				{
					GD.Print($"reset");
					ResetTurn();
				}
			}
		}

		private void MoveCharacterIfPossible()
		{
			if (!currentlyActioningCharacter.IsActive && currentlyActioningCharacter.Stats.AmountLeftToMoveThisTurn > 0)
			{
				var body = currentlyActioningCharacter.Body as CharacterKinematicBody;

				var target = new Vector3(1, 1, 1);

				var path = aStarNavigator.GetMovementPath(body.Transform.origin, target, currentlyActioningCharacter.Stats.AmountLeftToMoveThisTurn);

				GD.Print($"issuing path movement command");
				body.MoveWithPath(path);
				currentlyActioningCharacter.IsActive = true;
			}
			else if (currentlyActioningCharacter.IsActive && currentlyActioningCharacter.Stats.AmountLeftToMoveThisTurn == 0)
			{
				currentlyActioningCharacter.IsActive = false;
				currentlyActioningCharacter = null;
			}
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
			GD.Print($"Characted is {currentlyActioningCharacter.Stats.CharacterName}");
		}

		public override void _Ready()
		{
			aStarNavigator = GetNode<AStarNavigator>(AStarNavigator.GLOBAL_SCENE_PATH);
			base._Ready();
		}
	}
}
