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

	public void SetValueToCharactersCurrentHealth(Character character)
    {
		float percentage = (float)character.Stats.CurrentHealth / (float)character.Stats.MaxHealth;
		SetColour(percentage);
        Value = character.Stats.CurrentHealth;
        healthText.Text = $"{character.Stats.CurrentHealth}/{character.Stats.MaxHealth}";
        RectScale = new Vector2(percentage, RectScale.y);
    }

    private void SetColour(float percentage)
    {
        float red = 1.0f, green = 1.0f;

        if (percentage <= 0.5f)
        {
            red = 1f;
            green = percentage * 1.9f;
        }
        else
        {
            red =  (1 - percentage) * 1.9f;
			green = 1f;
        }

        TintProgress = new Color(red, green, 0, 1);
    }

    public void SetValuesToCharacterValues(Character character)
    {
        Hide();
        SetMaxHealth(character);
        SetValueToCharactersCurrentHealth(character);
        CallDeferred("show");
    }

    private void SetMaxHealth(Character character)
    {
        MaxValue = character.Stats.MaxHealth;
    }
}
