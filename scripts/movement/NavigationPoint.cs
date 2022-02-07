using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace FaffLatest.scripts.movement
{
    public class NavigationPoint
    {
        public Node OccupyingNode;

        public bool IsOccupied => OccupyingNode != null;
    }
}
