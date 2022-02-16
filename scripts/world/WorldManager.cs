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
		private Thread thread = new Thread();


		public SpawnManager SpawnManager { get => GetSpawnManager(); private set => spawnManager = value; }

		[Export]
		public CharacterGeneratorStats StatsForCharacterRandomiser { get; private set; }

		public CharacterRandomiser CharacterRandomiser { get; private set; }


		[Signal]
		public delegate void _Characters_Loaded();

		[Signal]
		public delegate void _World_Loaded();
		
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

			thread.Start(this, nameof(LoadCharacters), map);

            LoadWorldObjectsIntoAStar();

			await ToSignal(this, nameof(_Characters_Loaded));

			EmitSignal(nameof(_World_Loaded));
        }

        private void LoadWorldObjectsIntoAStar()
        {
            var worldObjects = GetTree().GetNodesInGroup("worldObjects");

            foreach (WorldObject worldObject in worldObjects)
            {
				worldObject.CallDeferred(nameof(WorldObject.RegisterWithAStar), astar);
            }
        }

        private void LoadCharacters(MapInfo map)
        {
            var shouldCreateRandomCharacters = map.Characters == null || map.Characters.Length == 0;

            if (shouldCreateRandomCharacters)
                InitialiseNewCharacters();
            else
                GetSpawnLocationsAndSpawnCharacters(map.Characters);
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

		public async void GetSpawnLocationsAndSpawnCharacters(CharacterStats[] characters)
		{
			var spawnLocations = this.GetSpawnArea();
			await SpawnManager.SpawnCharacters(characters, spawnLocations);
			EmitSignal(nameof(_Characters_Loaded));
		}

		
		private void InitialiseAStar(MeshInstance level)
		{
			astar = AStarNavigator.Instance;

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
	}
}
