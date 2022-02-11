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

        public MapInfo(PackedScene level = null, CharacterStats[] characters = null)
        {
            Level = level;
            Characters = characters;
        }

        [Export]
        public PackedScene Level { get; set; }

        [Export]
        public CharacterStats[] Characters { get; set; }
    }
}
