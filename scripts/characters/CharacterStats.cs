using FaffLatest.scripts.constants;
using Godot;
using System;

public class CharacterStats : Resource
{
	[Export]
	public string CharacterName { get; set; }

	[Export]
	public int Health { get; set; }

	[Export]
	public bool IsPlayerCharacter { get; set; }

	[Export]
	public Texture FaceIcon { get; set; }

	[Export]
	public int MaxMovementDistancePerTurn { get; private set; }


	public int AmountMovedThisTurn { get; private set; }

	public int AmountLeftToMoveThisTurn => MaxMovementDistancePerTurn - AmountMovedThisTurn;

	public bool FinishedTurnMovement => AmountMovedThisTurn == MaxMovementDistancePerTurn;
	public bool CanMove => !FinishedTurnMovement;

	public CharacterStats(string name = null, int health = 0, bool isPlayerCharacter = false, Texture faceIcon = null, int movemetDistance = 10)
	{
		CharacterName = name;
		Health = health;
		IsPlayerCharacter = isPlayerCharacter;
		FaceIcon = faceIcon;
		MaxMovementDistancePerTurn = movemetDistance;
	}

	public CharacterStats()
	{
	}

	public void SetName(string name)
	{
		this.CharacterName = name;
		//GD.Print(this.CharacterName);
	}

	public void SetPlayerCharacter(bool isPlayerCharacter)
	{
		IsPlayerCharacter = isPlayerCharacter;
		//GD.Print(this.IsPlayerCharacter);
	}

	public void SetMaxMovementDistance(int movementDistance) => MaxMovementDistancePerTurn = movementDistance;

	public void ResetMovement()
    {
		AmountMovedThisTurn = 0;
    }

	public void IncrementMovement()
    {
		AmountMovedThisTurn++;
		GD.Print($"Now moved {AmountMovedThisTurn} out of {MaxMovementDistancePerTurn}");
    }

	private void _On_Character_ReachedPathPart(Node character, Vector3 part)
    {
		IncrementMovement();
    }
}
