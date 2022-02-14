using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaffLatest.scripts.shared;
using Godot;

namespace FaffLatest.scripts.movement
{
    public class MovementStats : Resource
    {
        public MovementStats(
            float rotationSpeedInterval = 30.0f,
            float currentRotationSpeed = 0.0f,
            float maxRotationSpeed = 1.0f,
            float maxSpeed = 2f,
            Vector3 velocity = default(Vector3),
            float acceleration = 0.2f)
        {
            RotationSpeedInterval = rotationSpeedInterval;
            CurrentRotationSpeed = currentRotationSpeed;
            MaxRotationSpeed = maxRotationSpeed;
            MaxMovementSpeed = maxSpeed;
            Velocity = velocity;
            Acceleration = acceleration;
        }

        public MovementStats()
        {
        }

        [Export]
        public float RotationSpeedInterval { get; private set; }

        [Export]
        public float CurrentRotationSpeed { get; private set; }

        [Export]
        public float MaxRotationSpeed { get; private set; }

        [Export]
        public float MaxMovementSpeed { get; private set; }

        [Export]
        public Vector3 Velocity { get; private set; }

        [Export]
        public float Acceleration { get; private set; }

        [Export]
        public int MaxMovementDistancePerTurn { get; private set; }

        public int AmountMovedThisTurn { get; private set; } = 0;

        public int AmountLeftToMoveThisTurn => MaxMovementDistancePerTurn - AmountMovedThisTurn;

        public bool CanMove => AmountMovedThisTurn < MaxMovementDistancePerTurn;

        public void SetCurrentRotationSpeed(float newRotationSpeed)
            => CurrentRotationSpeed = Mathf.Clamp(newRotationSpeed, 0.0f, MaxRotationSpeed);

        public void SetMaxMovementDistance(int movementDistance) => MaxMovementDistancePerTurn = movementDistance;

        public void SetVelocity(Vector3 newVelocity)
        { 
            var clamped = Velocity.Clamp(newVelocity, MaxMovementSpeed);
            Velocity = clamped;
        }

        public void StopRotating() => CurrentRotationSpeed = 0.0f;
        public void StopMoving() => Velocity = Vector3.Zero;


        public void ResetMovement()
        {
            AmountMovedThisTurn = 0;
        }

        public void IncrementMovement()
        {
            AmountMovedThisTurn++;
        }

        private void _On_Character_ReachedPathPart(Node character, Vector3 part)
        {
            IncrementMovement();
        }
    }
}
