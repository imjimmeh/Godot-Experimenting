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
			target = new Vector3(target.x, body.Transform.origin.y, target.z);


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
        {
            return body.MovementStats.Velocity + body.Transform.origin;
        }

        public static Vector3 InterpolateAndMove(this KinematicBody body, float delta, Vector3 currentVelocity, Vector3 movementVector, float acceleration, float maxSpeed)
		{
			Vector3 newVelocity = CalculateNewVelocity(delta, currentVelocity, movementVector, acceleration);
			newVelocity = ClampVelocity(currentVelocity, maxSpeed, newVelocity);

			var desiredTransform = new Transform(body.Transform.basis, newVelocity + body.Transform.origin);
			body.Transform = body.Transform.InterpolateWith(desiredTransform, 1);

			return newVelocity;
		}

		public static Vector3 ClampVelocity(Vector3 currentVelocity, float maxSpeed, Vector3 newVelocity)
		{
			if (newVelocity.Length() > maxSpeed)
			{
				newVelocity = currentVelocity;
			}

			return newVelocity;
		}


		public static MovementStats CalculateAndSetNewVelocity(this MovementStats stats, Vector3 currentPosition, Vector3 target, float delta)
		{
			var movementVector = (target - currentPosition).Normalized();

			var newVelocity = CalculateNewVelocity(delta, stats.Velocity, movementVector, stats.Acceleration);
			stats.SetVelocity(newVelocity);

			return stats;
		}


		public static Vector3 CalculateNewVelocity(float delta, Vector3 currentVelocity, Vector3 movementVector, float acceleration)
		{
			return currentVelocity += movementVector * acceleration * delta;
		}

		public static bool IsCellWithinMovementDistance(this CharacterStats character, Vector3 pos, Vector3 currentVector)
		{
			var distanceToCharacter = (pos - currentVector).Abs();

			var distanceCalc = distanceToCharacter.x + distanceToCharacter.z;
			var withinDistance = character.MaxMovementDistancePerTurn >= distanceCalc;

			return withinDistance;
		}

	}
}
