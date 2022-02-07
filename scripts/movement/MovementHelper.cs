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
        public static (Vector3 newVelocity, KinematicCollision collision) InterpolateAndMove(this KinematicBody body, float delta, Vector3 currentVelocity, Vector3 movementVector, float acceleration, float maxSpeed)
        {
            Vector3 newVelocity = CalculateNewVelocity(delta, currentVelocity, movementVector, acceleration);
            newVelocity = ClampVelocity(currentVelocity, maxSpeed, newVelocity);

            return (newVelocity, body.MoveAndCollide(newVelocity));
        }

        public static Vector3 ClampVelocity(Vector3 currentVelocity, float maxSpeed, Vector3 newVelocity)
        {
            if (newVelocity.Length() > maxSpeed)
            {
                newVelocity = currentVelocity;
            }

            return newVelocity;
        }

        public static Vector3 CalculateNewVelocity(float delta, Vector3 currentVelocity, Vector3 movementVector, float acceleration)
        {
            return currentVelocity += movementVector * acceleration * delta;
        }
    }
}
