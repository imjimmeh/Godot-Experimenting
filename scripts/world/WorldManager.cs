using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.map;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.state;
using Godot;

namespace FaffLatest.scripts.world
{

	public class WorldManager : Spatial
	{
		private SpawnManager spawnManager;
		public SpawnManager SpawnManager { get => GetSpawnManager(); private set => spawnManager = value; }

		[Export]
		public CharacterGeneratorStats StatsForCharacterRandomiser { get; private set; }

		public CharacterRandomiser CharacterRandomiser { get; private set; }


		public override void _Ready()
		{
			base._Ready();
			CharacterRandomiser = new CharacterRandomiser(StatsForCharacterRandomiser);
		}

		private SpawnManager GetSpawnManager()
		{
			if(spawnManager == null)
			{
				spawnManager = GetNode<SpawnManager>(NodeReferences.Systems.SPAWN_MANAGER);
			}

			return spawnManager;
		}

		public void InitialiseMap(MapInfo map)
		{
			MeshInstance level = GetLevelInstance(map);

			InitialiseAStar(level);

			if(map.Characters == null || map.Characters.Length == 0)
				InitialiseNewCharacters();
			else
			{
				GetSpawnLocationsAndSpawnCharacters(map.Characters);
			}
		}

		private void InitialiseNewCharacters()
		{
			var chars = new CharacterStats[9];

			for (var x = 0; x < 9; x++)
			{
				var newCharacter = CharacterRandomiser.GenerateRandomCharacter();
				chars[x] = newCharacter;
				chars[x].IsPlayerCharacter = x <= 5;
				
			}

			GetSpawnLocationsAndSpawnCharacters(chars);
		}

		private void InitialiseAStar(MeshInstance level)
		{
			var aStar = GetNode<AStarNavigator>(AStarNavigator.GLOBAL_SCENE_PATH);

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

		private void GetSpawnLocationsAndSpawnCharacters(CharacterStats[] characters)
		{
			var spawnLocations = this.GetSpawnArea();
			SpawnManager.SpawnCharacters(characters, spawnLocations);
		}
	}
}
