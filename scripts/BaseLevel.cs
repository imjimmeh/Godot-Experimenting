using FaffLatest.scripts.characters;
using FaffLatest.scripts.map;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using FaffLatest.scripts.ui;
using Godot;
using System;

public class BaseLevel : Spatial
{
	[Export]
	public MapInfo Map { get; set; }

	[Export]
	public Texture FaceSprites { get; set; }

	public override void _Ready()
	{
		base._Ready();
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

		var environment = levelInstance as MeshInstance;
		var aStar = GetNode<AStarNavigator>("/root/Root/Systems/AStarNavigator");

		var plane = environment.Mesh as PlaneMesh;
		var size = plane.Size;

		aStar.CreatePointsForMap((int)size.x, (int)size.y, new Vector2[0]);


		var chars = new CharacterStats[5];

		Vector3[] pos = { new Vector3(1, 1, 1), new Vector3(3, 1, 3), new Vector3(3, 1, 1), new Vector3(5, 1, 3), new Vector3(3, 1, 5) };

		for (var x = 0; x < 5; x++)
		{
			var newCharacter = CharacterStatsGenerator.GenerateRandomCharacter();
			chars[x] = newCharacter;

		}

		var spawnManager = GetNode<SpawnManager>("/root/Root/Systems/SpawnManager");
		spawnManager.SpawnCharacters(chars, pos);


	}
}
