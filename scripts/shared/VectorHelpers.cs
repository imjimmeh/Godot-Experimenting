using System.Collections.Generic;
using FaffLatest.scripts.effects.movementguide;
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

        public static Vector3 CopyValues(this Vector3 vector, Vector3 target, bool copyX, bool copyY, bool copyZ)
            => new Vector3(
                x: copyX ? target.x : vector.x,
                y: copyY ? target.y : vector.y,
                z: copyZ ? target.z : vector.z);

        public static Vector3 CopyXValue(this Vector3 vector, Vector3 target)
            => vector.CopyValues(target, true, false, false);

        public static Vector3 CopyYValue(this Vector3 vector, Vector3 target)
            => vector.CopyValues(target, false, true, false);

        public static Vector3 CopyZValue(this Vector3 vector, Vector3 target)
            => vector.CopyValues(target, false, false, true);

        public static float DistanceToIgnoringHeight(this Vector3 origin, Vector3 target)
        {
            var distance = (target - origin).Abs();

            return distance.x + distance.z;
        }

        public static Vector3 MovementVectorTo(this Vector3 origin, Vector3 target)
            => (target - origin).Normalized();

        public static Vector3 WithValues(this Vector3 vector, float? x = null, float? y = null, float? z = null)
            => new Vector3(
                x ?? vector.x,
                y ?? vector.y,
                z ?? vector.z);

    }

    public class CharacterMovementGuideCellComparer : IEqualityComparer<CharacterMovementGuideCell>
    {
        public bool Equals(CharacterMovementGuideCell x, CharacterMovementGuideCell y)
        { return x.GlobalTransform.origin == y.GlobalTransform.origin;}

        public int GetHashCode(CharacterMovementGuideCell obj)
        { return obj.GlobalTransform.origin.GetHashCode() ^ obj.Transform.origin.GetHashCode();
    }
    }
}
