using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using FaffLatest.scripts.characters;
namespace FaffLatest.scripts.map
{
    public class MapInfo : Resource
    {
        [Export]
        public PackedScene Environment { get; set; }

        [Export]
        public Character[] Characters { get; set; }
    }
}
