using FaffLatest.scripts.characters;
using Godot;
using System;

public class ColouredBox : CSGBox
{
	public override void _Ready()
	{
		var character = GetNode<Character>("../../");
		var spatial = Material as ShaderMaterial;
		var isPlayerCharacter = character.Stats.IsPlayerCharacter;

		var colour = isPlayerCharacter ? new Vector3(1.0f, 0.0f, 0.0f) : new Vector3(0.0f, 1.0f, 0.0f);
		spatial.SetShaderParam("colour", colour);
		spatial.SetShaderParam("alpha", "1.0f");

	}
}
