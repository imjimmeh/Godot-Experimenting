using Godot;
using System;

namespace FaffLatest.scripts.effects
{
    public static class MovementPathFactory
    {
		private static PlaneMesh CreatePlaneForPoint(Vector3 point, Action<PlaneMesh> options)
		{
			var plane = new PlaneMesh();
			plane.Size = new Vector2(1, 1);

			if (options != null)
			{
				options(plane);
			}

			return plane;
		}

		public static CharacterMovementPath CreateMeshInstanceForPoint(Vector3 point, Action<PlaneMesh> options = null)
		{
			var meshInstance = new CharacterMovementPath();
			meshInstance.Mesh = CreatePlaneForPoint(point, options);

			meshInstance.Transform = new Transform(meshInstance.Transform.basis, point);
			return meshInstance;
		}

	}
}
