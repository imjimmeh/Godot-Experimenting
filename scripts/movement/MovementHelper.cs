using FaffLatest.scripts.shared;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.movement
{
	public static class MovementHelper
	{
		public static MovingKinematicBody InterpolateAndMove(this MovingKinematicBody body, float delta, Vector3 target)
        {
			target = target.CopyYValue(body.Transform.origin);

			body.MovementStats.CalculateAndSetNewVelocity(body.Transform.origin, target, delta);
            CalculateAndSetTransform(body);

			return body;
        }

        private static void CalculateAndSetTransform(MovingKinematicBody body)
        {
            var desiredTransform = new Transform(body.Transform.basis, GetTargetPosition(body));
            body.Transform = body.Transform.InterpolateWith(desiredTransform, 1);
        }

        private static Vector3 GetTargetPosition(MovingKinematicBody body)
			=> body.MovementStats.Velocity + body.Transform.origin;

		public static MovementStats CalculateAndSetNewVelocity(this MovementStats stats, Vector3 currentPosition, Vector3 target, float delta)
		{
			var movementVector = currentPosition.MovementVectorTo(target);

			var newVelocity = CalculateNewVelocity(delta, stats.Velocity, movementVector, stats.Acceleration);
			stats.SetVelocity(newVelocity);

			return stats;
		}

		public static Vector3 CalculateNewVelocity(float delta, Vector3 currentVelocity, Vector3 movementVector, float acceleration)
			=> currentVelocity += movementVector * acceleration * delta;

		public static bool IsCellWithinMovementDistance(this MovementStats movementStats, Vector3 pos, Vector3 currentVector)
		{
			var distance = currentVector.DistanceToIgnoringHeight(pos);
			var withinDistance = movementStats.MaxMovementDistancePerTurn >= distance;

			return withinDistance;
		}
	}
}
