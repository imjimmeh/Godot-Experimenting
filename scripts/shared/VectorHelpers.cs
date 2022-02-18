using System.Collections.Generic;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.effects.movementguide;
using Godot;

namespace FaffLatest.scripts.shared
{
    public static class VectorHelpers
    {
        public static bool AnyValueLessThanZero(this Vector3 vector)
            => vector.x < 0 || vector.z < 0;

        public static bool AnyValueGreaterThanOrEqualToValue(this Vector3 vector, float v)
            => vector.x >= v || vector.z >= v;

        public static bool LookingAtSamePoint(Vector3 backwardsVector, Vector3 movementVector)
        {
            var dotProduct = movementVector.Dot(backwardsVector);

            var lookingAtDestination = Mathf.IsEqualApprox(dotProduct, -1f);

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

        public static float NonEuclideanDistanceToIgnoringHeight(this Vector3 origin, Vector3 target)
        {
            var distance = (target - origin).Abs();

            return distance.x + distance.z;
        }
        
        public static float DistanceToIgnoringHeight(this Vector3 origin, Vector3 target)
        {
            var withSameY = origin.CopyYValue(target);
            return origin.DistanceTo(target);
        }

        public static Vector3 MovementVectorTo(this Vector3 origin, Vector3 target)
            => (target - origin).Normalized();

        public static Vector3 WithValues(this Vector3 vector, float? x = null, float? y = null, float? z = null)
            => new Vector3(
                x ?? vector.x,
                y ?? vector.y,
                z ?? vector.z);
    }
}
