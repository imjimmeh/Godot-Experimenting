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
			//var rect = new Rect2(new Vector2(topLeft.x, topLeft.z), new Vector2(fullSize.x, fullSize.z));

			//float upperX = Mathf.Ceil(rect.Position.x + rect.Size.x);
			//float upperY = Mathf.Ceil(rect.Position.y + rect.Size.y);

			//GD.Print($"{extents} - {fullSize} - {topLeft} - {rect} - {upperX}");
			for (var x = 0; x < fullSize.x; x++)
			{
				for (var y = 0; y < fullSize.z; y++)
				{
					var point = new Vector3(topLeft.x + x, 0, topLeft.z + y);
					GD.Print($"Disabling point for world object {Name} at {point}");
					yield return point;
				}
			}
		}
	}
}
