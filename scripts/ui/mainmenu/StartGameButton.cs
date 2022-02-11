using FaffLatest.scripts.map;
using Godot;
using System;

namespace FaffLatest.scripts.ui.mainmenu
{
    public class StartGameButton : Button
    {
        private LevelLoader levelLoader;

        [Export]
        public MapInfo FirstLevel { get;set; }

        public override void _Ready()
        {
            levelLoader = GetNode<LevelLoader>("/root/LevelLoader");
            base._Ready();

            if (FirstLevel == null || FirstLevel.Level == null)
                throw new Exception($"No level specified to start");
        }

        public override void _Pressed()
        {
            base._Pressed();
            levelLoader.LoadLevel(FirstLevel);
        }
    }
}