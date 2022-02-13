using FaffLatest.scripts.characters;
using Godot;
using System;

public class HealthBar : TextureProgress
{
	public override void _Process(float delta)
	{
		base._Process(delta);
	}

	
	public override void _Ready()
	{
		
	}

	public void CharacterSelectionCleared()
    {
		Value = 0;
		MaxValue = 0;
    }

	public void SetValueToCharactersCurrentHealth(Character character)
		=> Value = character.Stats.CurrentHealth;

	public void SetValuesToCharacterValues(Character character)
    {
		SetValueToCharactersCurrentHealth(character);
		MaxValue = character.Stats.MaxHealth;
    }
}
