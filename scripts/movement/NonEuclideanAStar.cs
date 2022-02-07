using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.movement
{
    public class NonEuclideanAStar : AStar
    {
        public override float _ComputeCost(int fromId, int toId)
        {
            return _EstimateCost(fromId, toId);
        }

        public override float _EstimateCost(int fromId, int toId)
        {
            return 1;
        }
    }
}
