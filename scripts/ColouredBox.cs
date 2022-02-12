using FaffLatest.scripts.characters;
using Godot;
using System;

public class ColouredBox : CSGBox
{
	public override void _Ready()
	{
		//spatial.SetShaderParam("colour", colour);
		//spatial.SetShaderParam("alpha", "1.0f");
		base._Ready();
	}

	public void SetColour(CharacterStats stats)
    {
		var material = Material as SpatialMaterial;
		var isPlayerCharacter = stats.IsPlayerCharacter;

		var colour = isPlayerCharacter ? new Color(1.0f, 0.0f, 0.0f, 1.0f) : new Color(0.0f, 1.0f, 0.0f, 1.0f);

		material.AlbedoColor = colour;
	}
}
