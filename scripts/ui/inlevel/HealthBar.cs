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
		Value = character.Stats.CurrentHealth;
		healthText.Text = $"{character.Stats.CurrentHealth}/{character.Stats.MaxHealth}";

		float percentage = (float)character.Stats.CurrentHealth/(float)character.Stats.MaxHealth;
		RectScale = new Vector2(percentage, RectScale.y);

		TintProgress = new Color(percentage > 75 ? 0 : 1, percentage > 50 ? 1 : percentage, 0, 1);
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
