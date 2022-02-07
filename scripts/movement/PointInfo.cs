using System;
using Godot;

namespace FaffLatest.scripts.movement
{
    public class PointInfo
    {
        private Node _occupyingNode;

        public PointInfo()
        {
        }

        public PointInfo(int id)
        {
            Id = id;
        }

        public PointInfo(int id, Node occupyingNode)
        {
            Id = id;
            OccupyingNode = occupyingNode;
        }

        public int Id { get; set; }

        public Node OccupyingNode { get => _occupyingNode; private set => _occupyingNode = value; }

        public (Node oldOccupant, Node newOccupant) SetOccupier(Node newOccupier)
        {
            var old = OccupyingNode;
            OccupyingNode = newOccupier;

            return (old, OccupyingNode);
        }
    }
}
