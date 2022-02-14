using FaffLatest.scripts.constants;
using FaffLatest.scripts.weapons;
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
	public Weapon EquippedWeapon { get; set; }

	public int CurrentHealth { get; private set; }

	public bool HasUsedActionThisTurn { get; private set; }


	public CharacterStats(string name = null, int maxHealth = 0, bool isPlayerCharacter = false, Texture faceIcon = null, Weapon weapon = null)
    {
        Initialise(name, maxHealth, isPlayerCharacter, faceIcon, weapon);
    }

    public CharacterStats Initialise(string name, int maxHealth, bool isPlayerCharacter, Texture faceIcon, Weapon weapon)
    {
        CharacterName = name;
        MaxHealth = maxHealth;
        IsPlayerCharacter = isPlayerCharacter;
        FaceIcon = faceIcon;
        CurrentHealth = maxHealth;
		EquippedWeapon = weapon;

		return this;
    }

    public CharacterStats()
	{
	}

	public void SetCharacterName(string name)
	{
		this.CharacterName = name;
		//GD.Print(this.CharacterName);
	}

	public void SetPlayerCharacter(bool isPlayerCharacter)
	{
		IsPlayerCharacter = isPlayerCharacter;
		//GD.Print(this.IsPlayerCharacter);
	}

	public void SetWeapon(Weapon weapon)
	{
		EquippedWeapon = weapon;
	}

	public void AddHealth(int health) => CurrentHealth += health;

	public void SetHealthAsMax() => CurrentHealth = MaxHealth;

	public override string ToString() => $"{this.CharacterName}, max health: {MaxHealth}, currentHealth: {CurrentHealth}, isPC: {IsPlayerCharacter}, Weapon: {EquippedWeapon?.Name} ";
}
