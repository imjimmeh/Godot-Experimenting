using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.world
{
	public class WorldObject : Spatial
	{
		public override void _Ready()
		{
			base._Ready();
			AddToGroup("worldObjects");
		}

		public void NotifyAStarNavigator()
		{

		}

		public void RegisterWithAStar(AStarNavigator astar)
		{
			foreach(var cell in GetOccupiedCells())
			{
				astar.MarkNodeAsOccupied(cell);
			}
		}

		public IEnumerable<Vector3> GetOccupiedCells()
		{
			var collisionObject = GetNode<CollisionShape>("CollisionShape");

			var shape = collisionObject.Shape;
			switch (shape)
			{
				case BoxShape box:
					{
						return GetOccupiedCellsForBoxShape(box);
					}
			}

			return null;
		}

		public IEnumerable<Vector3> GetOccupiedCellsForBoxShape(BoxShape box)
		{
			var extents = box.Extents * Scale;
			var fullSize = extents * 2.0f;
			var topLeft = (GlobalTransform.origin - extents).Ceil();

			extents = extents.Ceil();

			for (var x = 0; x < fullSize.x; x++)
			{
				for (var y = 0; y < fullSize.z; y++)
				{
					yield return new Vector3(
						x: topLeft.x + x,
						y: 0, 
						z: topLeft.z + y);
				}
			}
		}
	}
}
