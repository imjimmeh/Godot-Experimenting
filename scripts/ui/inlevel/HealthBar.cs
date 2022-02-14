using FaffLatest.scripts.characters;
using Godot;
using System;

public class HealthBar : TextureProgress
{
	[Export]
	public DynamicFont HealthTextFont { get; set; }

	private Label healthText;

	public override void _Ready()
    {
        healthText = GetNode<Label>("HealthText");

        SetFont();

        base._Ready();
    }

    private void SetFont()
    {
		HealthTextFont.Size = 24;
		HealthTextFont.OutlineColor = Colors.White;
		HealthTextFont.OutlineSize = 1;

		healthText.AddFontOverride("font", HealthTextFont);
		healthText.AddColorOverride("font_color", Colors.Black);
    }

    public override void _Process(float delta)
	{
		base._Process(delta);
	}

	public void CharacterSelectionCleared()
    {
		Value = 0;
		MaxValue = 0;
    }

	public void SetValueToCharactersCurrentHealth(Character character)
	{
		Value = character.Stats.CurrentHealth;
		healthText.Text = $"{character.Stats.CurrentHealth}/{character.Stats.MaxHealth}";
	}

	public void SetValuesToCharacterValues(Character character)
    {
		SetValueToCharactersCurrentHealth(character);
		MaxValue = character.Stats.MaxHealth;

	}
}
