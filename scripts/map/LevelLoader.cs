using System;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.state;
using Godot;


namespace FaffLatest.scripts.map
{
    public class LevelLoader : Godot.Node
    {
        [Export]
        public PackedScene BaseLevel;

        public void LoadLevel(MapInfo map)
        {
            var baseLevel = BaseLevel.Instance() as BaseLevel;


            var root = GetNode("/root");

            var currentScene = root.GetChild(0);
            root.AddChild(baseLevel);

            baseLevel.Map = map;
            baseLevel.LoadMap(map); ;
            root.RemoveChild(currentScene);

            currentScene.Dispose();
        }
    }
}
