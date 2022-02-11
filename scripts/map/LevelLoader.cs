using System;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.state;
using Godot;


namespace FaffLatest.scripts.map
{
	public class LevelLoader : Godot.Node
	{
		[Export]
		public PackedScene BaseLevel { get; set; }

		private bool currentLevelIsDisposing = false;

        private Node currentScene { get; set; }
        private Node root { get; set; }
        private MapInfo loadingMap { get; set; 
        }
        public override void _Ready()
        {
            base._Ready();
            Initialise();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if(currentLevelIsDisposing)
            {
                DisposeLevel();
            }
        }

        private void Initialise()
        {
            root = GetNode("/root");
        }


        public void LoadLevel(MapInfo map)
        {
            ClearLevel();
            loadingMap = map;
        }

        private void ClearLevel()
        {
            currentScene = root.GetChild(1);
            currentScene.QueueFree();
            currentLevelIsDisposing = true;
        }

        private void DisposeLevel()
        {
            //root.RemoveChild(currentScene);
            //currentScene.Dispose();

            var baseLevel = BaseLevel.Instance() as BaseLevel;
            root.AddChild(baseLevel);
            baseLevel.LoadMap(loadingMap);

            ClearVariables();
        }

        private void ClearVariables()
        {
            currentLevelIsDisposing = false;
            loadingMap = null;
        }
    }
}
