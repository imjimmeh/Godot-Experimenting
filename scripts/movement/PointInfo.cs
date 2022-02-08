using System;
using Godot;

namespace FaffLatest.scripts.movement
{
	public class PointInfo : Godot.Object
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

			GD.Print($"{newOccupier} is now occupying {Id}");
			return (old, OccupyingNode);
		}

		public override string ToString()
		{
			return $"{{Id:{Id}, OccupyingNode:{OccupyingNode?.Name}}}";
		}
	}
}
