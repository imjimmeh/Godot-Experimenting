using Godot;

namespace FaffLatest.scripts.shared
{
    public static class VectorHelpers
    {
        public static bool LookingAtSamePoint(Vector3 backwardsVector, Vector3 movementVector)
        {
            var dotProduct = movementVector.Dot(backwardsVector);

            var lookingAtDestination = dotProduct < -0.9999f;
            return lookingAtDestination;
        }
    }
}
