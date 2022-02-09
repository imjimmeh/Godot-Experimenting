using FaffLatest.scripts.constants;
using Godot;
using System;

public class CharacterStats : Resource
{
	[Export]
	public string CharacterName { get; private set; }

	[Export]
	public int MaxHealth { get; private set; }

	[Export]
	public bool IsPlayerCharacter { get; set; }

	[Export]
	public Texture FaceIcon { get; private set; }

	[Export]
	public int MaxMovementDistancePerTurn { get; private set; }


	public int AmountMovedThisTurn { get; private set; }

	public int AmountLeftToMoveThisTurn => MaxMovementDistancePerTurn - AmountMovedThisTurn;

	public bool FinishedTurnMovement => AmountMovedThisTurn >= MaxMovementDistancePerTurn;
	public bool CanMove => !FinishedTurnMovement;

	public int CurrentHealth { get; private set; }

	public bool HasUsedActionThisTurn { get; private set; }

	public CharacterStats(string name = null, int maxHealth = 0, bool isPlayerCharacter = false, Texture faceIcon = null, int movementDistance = 10)
    {
        Initialise(name, maxHealth, isPlayerCharacter, faceIcon, movementDistance);
    }

    public void Initialise(string name, int maxHealth, bool isPlayerCharacter, Texture faceIcon, int movementDistance)
    {
        CharacterName = name;
        MaxHealth = maxHealth;
        IsPlayerCharacter = isPlayerCharacter;
        FaceIcon = faceIcon;
        MaxMovementDistancePerTurn = movementDistance;
        CurrentHealth = maxHealth;
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
		//GD.Print($"Now moved {AmountMovedThisTurn} out of {MaxMovementDistancePerTurn}");
    }

	public void AddHealth(int health) => CurrentHealth += health;

	private void _On_Character_ReachedPathPart(Node character, Vector3 part)
    {
		IncrementMovement();
    }

	public override string ToString() => $"{this.CharacterName}, health: {MaxHealth}, isPC: {IsPlayerCharacter}, movementLeft: {AmountLeftToMoveThisTurn}";
}
