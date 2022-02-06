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
	public float MovementDistance { get; set; }

	public CharacterStats(string name = null, int health = 0, bool isPlayerCharacter = false, Texture faceIcon = null, float movemetDistance = 10.0f)
	{
		CharacterName = name;
		Health = health;
		IsPlayerCharacter = isPlayerCharacter;
		FaceIcon = faceIcon;
		MovementDistance = movemetDistance;
	}

	public CharacterStats()
	{

	}

	public void WithName(string name)
	{
		this.CharacterName = name;
		GD.Print(this.CharacterName);
	}

	public void SetPlayerCharacter(bool isPlayerCharacter)
	{
		IsPlayerCharacter = isPlayerCharacter;
		GD.Print(this.IsPlayerCharacter);
	}
}
