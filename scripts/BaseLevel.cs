using FaffLatest.scripts.map;
using Godot;
using System;

public class BaseLevel : Spatial
{
	[Export]
	public MapInfo Map { get; set; }

	public override void _Ready()
	{
		if (Map == null)
			return;
	}

	public void LoadMap(MapInfo map)
	{
		Map = map;

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
