using FaffLatest.scripts.characters;
using Godot;
using System;

public class HealthBar : TextureProgress
{
	public Character Character;

	public override void _Process(float delta)
	{
		base._Process(delta);

		if(!this.Visible || Character == null)
		{
			Value = 0;
			return;
		}

		Value = Character.Stats.MaxHealth;
	}

	
	public override void _Ready()
	{
		
	}

}
