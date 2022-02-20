using FaffLatest.scripts.movement;
using Godot;
using System;
using System.Collections.Generic;

namespace FaffLatest.scripts.world
{
    public class WorldObject : StaticBody
	{
		private SpatialMaterial material;

		public async override void _Ready()
		{
			base._Ready();
			AddToGroup("worldObjects");

			await ToSignal(GetParent(), "ready");
			try
			{
			material = GetParent<MeshInstance>().Mesh.SurfaceGetMaterial(0) as SpatialMaterial;

			Connect("mouse_entered", this, "_On_MouseEntered");
			Connect("mouse_exited", this, "_On_MouseExited");
			}
			catch(Exception ex)
			{
				GD.Print($"{ex.Message} - {Name}");
			}

		}

		public void NotifyAStarNavigator()
		{

		}

		public void RegisterWithAStar(AStarNavigator astar)
		{
			foreach(var cell in GetOccupiedCells())
			{
				astar.AStar.MarkNodeAsOccupied(cell);
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
				case ConcavePolygonShape concavePolygonShape:
					{
						return GetOccupiedCellsForConcavePolygonShape(concavePolygonShape);
					}
			}

			return null;
		}

		public IEnumerable<Vector3> GetOccupiedCellsForConcavePolygonShape(ConcavePolygonShape concavePolygonShape)
		{
			var data = concavePolygonShape.Data;

			HashSet<Vector3> foundPositions = new HashSet<Vector3>();
			foreach(var poly in data)
			{
				if(Mathf.IsZeroApprox(poly.y))
				{
					var position = poly + GlobalTransform.origin;
					position = position.Round();
					
					if(!foundPositions.Contains(position))
						foundPositions.Add(position);
				}
			}

			return foundPositions;
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

		private void _On_MouseEntered()
		{
			material.FlagsTransparent = true;
			material.AlbedoColor = new Color(1, 1, 1, 0.5f);
		}

		private void _On_MouseExited()
		{
			material.FlagsTransparent = true;
			material.AlbedoColor = new Color(1, 1, 1, 1);
		}
	}
}
