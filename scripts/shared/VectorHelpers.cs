using Godot;

namespace FaffLatest.scripts.shared
{
    public static class VectorHelpers
    {
        public static bool LookingAtSamePoint(Vector3 backwardsVector, Vector3 movementVector)
        {
            var dotProduct = movementVector.Dot(backwardsVector);

            var lookingAtDestination = dotProduct < -0.99f;

            return lookingAtDestination;
        }
        public static Vector3 Clamp(this Vector3 current, Vector3 newVal, float max)
                => newVal.Length() > max ? current : newVal;

        public static Vector3 ClampMax(Vector3 newVal, float max) => newVal.Normalized() * max;

    }
}
