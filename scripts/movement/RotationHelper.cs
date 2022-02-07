using FaffLatest.scripts.shared;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.movement
{
    public static class RotationHelper
    {	
		public static (Transform newTransform, float newRotationSpeed) InterpolateAndRotate(this Transform transform, float delta, float currentRotationSpeed, float rotationSpeedInterval, float maxRotationSpeed, Transform rotationTarget)
        { 
            var newRotationSpeed = CalculateNewRotationSpeed(delta, currentRotationSpeed, rotationSpeedInterval, maxRotationSpeed);

            var newTransform = transform.InterpolateWith(rotationTarget, newRotationSpeed);

            return (newTransform, newRotationSpeed);
        }

        public static float CalculateNewRotationSpeed(float delta, float currentRotationSpeed, float rotationSpeedInterval, float maxRotationSpeed)
        {
            currentRotationSpeed += delta * rotationSpeedInterval;

            if (currentRotationSpeed > maxRotationSpeed)
                currentRotationSpeed = maxRotationSpeed;

            return currentRotationSpeed;
        }

        public static bool CurrentRotationMatchesTarget(this Transform transform, Vector3 movementVector) 
            => VectorHelpers.LookingAtSamePoint(transform.basis.z.Normalized(), movementVector);

    }
}
