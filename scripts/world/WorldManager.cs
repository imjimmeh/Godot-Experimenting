using FaffLatest.scripts.characters;
using FaffLatest.scripts.map;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using Godot;
using System;

namespace FaffLatest.scripts.world
{

	public class WorldManager : Spatial
	{
		public override void _Ready()
		{

		}

		public void InitialiseMap(MapInfo map)
		{
			MeshInstance level = GetLevelInstance(map);

			InitialiseAStar(level);

			InitialiseCharacters();
		}

		private void InitialiseCharacters()
		{
			var chars = new CharacterStats[9];

			Vector3[] pos = { new Vector3(1, 1, 1), new Vector3(3, 1, 3), new Vector3(3, 1, 1), new Vector3(5, 1, 3), new Vector3(3, 1, 5),
			new Vector3(12, 1, 17), new Vector3(17, 1, 17) , new Vector3(15, 1, 17) , new Vector3(17, 1, 12)  };

			for (var x = 0; x < 9; x++)
			{
				var newCharacter = CharacterStatsGenerator.GenerateRandomCharacter();
				chars[x] = newCharacter;

				if (x >= 5)
					chars[x].IsPlayerCharacter = false;
			}

			var spawnManager = GetNode<SpawnManager>("/root/Root/Systems/SpawnManager");
			spawnManager.SpawnCharacters(chars, pos);
		}

		private void InitialiseAStar(MeshInstance level)
		{
			var aStar = GetNode<AStarNavigator>("/root/Root/Systems/AStarNavigator");

			var plane = level.Mesh as PlaneMesh;
			var size = plane.Size;

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
