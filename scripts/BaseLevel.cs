using FaffLatest.scripts.characters;
using FaffLatest.scripts.map;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using FaffLatest.scripts.ui;
using FaffLatest.scripts.world;
using Godot;
using System;

public class BaseLevel : Spatial
{ 
	[Export]
	public Texture FaceSprites { get; set; }

	public override void _Ready()
	{
		base._Ready();
	}

	public void LoadMap(MapInfo map)
	{
		var worldManager = GetNode<WorldManager>("/root/Root/Environment");

		worldManager.InitialiseMap(map);

	}
}
