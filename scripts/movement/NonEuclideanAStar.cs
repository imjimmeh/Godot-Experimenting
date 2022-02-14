using Godot;

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
            var from = GetPointPosition(fromId);
            var to = GetPointPosition(toId);

            int cost = 0;

            if (from.x != to.x)
                cost++;

            if (from.z != to.z)
                cost++;

            return cost;
        }
    }
}
