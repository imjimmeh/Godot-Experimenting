﻿using FaffLatest.scripts.shared;
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
        public static Transform InterpolateRotation(this MovingKinematicBody mover, Vector3 target)
        {
            var lookingAt = mover.Transform.LookingAt(target, Vector3.Up);

            return mover.Transform.InterpolateWith(lookingAt, mover.MovementStats.CurrentRotationSpeed);
        }

        public static MovingKinematicBody CalculateRotation(this MovingKinematicBody mover, float delta)
        {
            mover.CalculateAndSetNewRotationSpeed(delta);
            return mover;
        }

        public static (Transform newTransform, float newRotationSpeed) InterpolateAndRotate(this Transform transform, float delta, float currentRotationSpeed, float rotationSpeedInterval, float maxRotationSpeed, Transform rotationTarget)
        { 
            var newRotationSpeed = CalculateNewRotationSpeed(delta, currentRotationSpeed, rotationSpeedInterval, maxRotationSpeed);

            var newTransform = transform.InterpolateWith(rotationTarget, newRotationSpeed);

            return (newTransform, newRotationSpeed);
        }

        public static MovingKinematicBody CalculateAndSetNewRotationSpeed(this MovingKinematicBody mover, float delta)
        {
            var newSpeed = CalculateNewRotationSpeed(delta, mover.MovementStats.CurrentRotationSpeed, mover.MovementStats.RotationSpeedInterval, mover.MovementStats.MaxRotationSpeed);
            mover.MovementStats.SetCurrentRotationSpeed(newSpeed);

            return mover;
        }

        public static float CalculateNewRotationSpeed(float delta, float currentRotationSpeed, float rotationSpeedInterval, float maxRotationSpeed)
        {
            currentRotationSpeed += delta * rotationSpeedInterval;

            return currentRotationSpeed;
        }

        public static bool CurrentRotationMatchesTarget(this Transform transform, Vector3 movementVector) 
            => VectorHelpers.LookingAtSamePoint(transform.basis.z.Normalized(), movementVector);
    }
}
