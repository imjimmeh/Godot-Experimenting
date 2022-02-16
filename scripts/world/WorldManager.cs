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
		private AStarNavigator astar;
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

		public async void InitialiseMap(MapInfo map)
		{
			MeshInstance level = GetLevelInstance(map);

			InitialiseAStar(level);

			await ToSignal(level, "ready");

			if(map.Characters == null || map.Characters.Length == 0)
				InitialiseNewCharacters();
			else
			{
				GetSpawnLocationsAndSpawnCharacters(map.Characters);
			}

			var worldObjects = GetTree().GetNodesInGroup("worldObjects");

			foreach(var worldObject in worldObjects)
            {
				if(worldObject is WorldObject wo)
                {
					wo.RegisterWithAStar(astar);
                }
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
			astar = GetNode<AStarNavigator>(AStarNavigator.GLOBAL_SCENE_PATH);

			var plane = level.Mesh as PlaneMesh;
			var size = plane.Size;

			astar.CreatePointsForMap((int)size.x, (int)size.y, new Vector2[0]);
		}

		private MeshInstance GetLevelInstance(MapInfo map)
		{
			var levelInstance = map.Level.Instance();
			CallDeferred("add_child",levelInstance);

			return levelInstance as MeshInstance;
		}

		public void GetSpawnLocationsAndSpawnCharacters(CharacterStats[] characters)
		{
			var spawnLocations = this.GetSpawnArea();
			SpawnManager.SpawnCharacters(characters, spawnLocations);
		}
	}
}
