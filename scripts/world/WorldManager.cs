using FaffLatest.scripts.characters;
using FaffLatest.scripts.map;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using Godot;
using System;
using FaffLatest.scripts.map;
using System.Linq;

namespace FaffLatest.scripts.world
{

	public class WorldManager : Spatial
	{

		public override void _Ready()
		{
			base._Ready();
		}

		public void InitialiseMap(MapInfo map)
		{
			MeshInstance level = GetLevelInstance(map);

			InitialiseAStar(level);

			if(map.Characters == null || map.Characters.Length == 0)
				InitialiseCharacters();
			else
            {
				Vector3[] pos = { new Vector3(1, 1, 1), new Vector3(3, 1, 3), new Vector3(3, 1, 1), new Vector3(5, 1, 3), new Vector3(3, 1, 5), new Vector3(9, 1, 9)};

				var asList = pos.ToList();
				var spawnArea = new SpawnableAreas(asList, asList);
				var spawnManager = GetNode<SpawnManager>("/root/Root/Systems/SpawnManager");
				spawnManager.SpawnCharacters(map.Characters, spawnArea);

			}
		}

		private void InitialiseCharacters()
		{
			var chars = new CharacterStats[9];

			var spawnLocations = this.GetSpawnArea();

			for (var x = 0; x < 9; x++)
			{
				var newCharacter = CharacterStatsGenerator.GenerateRandomCharacter();
				chars[x] = newCharacter;

				if (x >= 5)
					chars[x].IsPlayerCharacter = false;
			}

			var spawnManager = GetNode<SpawnManager>("/root/Root/Systems/SpawnManager");
			spawnManager.SpawnCharacters(chars, spawnLocations);
		}

		private void InitialiseAStar(MeshInstance level)
		{
			var aStar = GetNode<AStarNavigator>("/root/Root/Systems/AStarNavigator");

			var plane = level.Mesh as PlaneMesh;
			var size = plane.Size;

			GD.Print($"intialising world of size {size.x}, {size.y}");
			aStar.CreatePointsForMap((int)size.x, (int)size.y, new Vector2[0]);
		}

		private MeshInstance GetLevelInstance(MapInfo map)
		{
			var levelInstance = map.Level.Instance();
			AddChild(levelInstance);

			return levelInstance as MeshInstance;
		}
	}
}
