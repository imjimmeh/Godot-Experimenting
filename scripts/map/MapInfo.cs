using FaffLatest.scripts.characters;
using Godot;
using System;

namespace FaffLatest.scripts.map
{
    public class MapInfo : Resource
    {
        public MapInfo()
        {
        }

        public MapInfo(PackedScene level = null)
        {
            Level = level;
        }

        [Export]
        public PackedScene Level { get; set; }
    }
}
