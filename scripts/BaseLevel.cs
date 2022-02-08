using FaffLatest.scripts.map;
using Godot;
using System;

public class BaseLevel : Spatial
{
	[Export]
	public MapInfo Map { get; set; }

	public override void _Ready()
	{
		Map.ResourcePath = "res://resources/levels/level1.tres";
		GD.Print(Map.Level.ResourcePath);
		LoadMap();
	}

	public void LoadMap()
	{

		var baseNodeForLevel = GetNode("Environment");

		var levelInstance = Map.Level.Instance();
		baseNodeForLevel.AddChild(levelInstance);
		levelInstance.RequestReady();
		GD.Print(Map.ResourcePath);

		var children = baseNodeForLevel.GetChild(0) as Spatial;
		GD.Print($"Found {children.Transform.origin}");
		children.Show();

	}
}
