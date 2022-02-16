using System;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.state;
using Godot;


namespace FaffLatest.scripts.map
{
	public class LevelLoader : Godot.Node
	{
		[Export]
		public PackedScene BaseLevelScene { get; set; }

		private bool currentLevelIsDisposing = false;
		private bool startedLevelLoad = false;

		private Node currentScene { get; set; }
		private Node root { get; set; }
		private MapInfo loadingMap { get; set; }
		
		public override void _Ready()
		{
			base._Ready();
			Initialise();
		}

		public override void _Process(float delta)
		{
			base._Process(delta);

			if(currentLevelIsDisposing && !startedLevelLoad)
			{
				startedLevelLoad = true;
				LoadLevel();
			}
		}

		private void Initialise()
		{
			root = GetNode("/root");
		}


		public void StartLevelLoading(MapInfo map)
		{
			InitialiseCurrentSceneDisposal();
			loadingMap = map;
		}

		private void InitialiseCurrentSceneDisposal()
		{
			currentScene = root.GetChild(1);
			currentLevelIsDisposing = true;
		}

		private async void LoadLevel()
        {
            BaseLevel baseLevel = GetBaseLevelInstance();
            LoadMapIntoBaseLevel(baseLevel);
            ClearVariables();

            await DisposeOriginalSceneOnLevelLoad(baseLevel);
        }

        private async Task DisposeOriginalSceneOnLevelLoad(BaseLevel baseLevel)
        {
            await ToSignal(baseLevel, nameof(BaseLevel._Level_Loaded));
            currentScene.CallDeferred("queue_free");
        }

        private void LoadMapIntoBaseLevel(BaseLevel baseLevel)
        {
            baseLevel.CallDeferred(nameof(BaseLevel.LoadLevel), loadingMap);
        }

        private BaseLevel GetBaseLevelInstance()
        {
            var baseLevel = BaseLevelScene.Instance() as BaseLevel;
            root.CallDeferred("add_child", baseLevel);
            return baseLevel;
        }

        private void ClearVariables()
		{
			currentLevelIsDisposing = false;
			loadingMap = null;
			
		}
	}
}
