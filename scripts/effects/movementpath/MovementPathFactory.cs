using Godot;
using System;

namespace FaffLatest.scripts.effects
{
    public static class MovementPathFactory
    {
		private static PlaneMesh CreatePlaneForPoint(Vector3 point, Action<PlaneMesh> options)
		{
            var plane = new PlaneMesh
            {
                Size = new Vector2(1, 1)
            };

            options?.Invoke(plane);

            return plane;
		}

		public static CharacterMovementPath CreateMeshInstanceForPoint(Vector3 point, Action<PlaneMesh> options = null)
		{
            GD.Print("Created mesh");
            var meshInstance = new CharacterMovementPath
            {
                Mesh = CreatePlaneForPoint(point, options)
            };

            meshInstance.Transform = new Transform(meshInstance.Transform.basis, point);

			return meshInstance;
		}

	}
}
