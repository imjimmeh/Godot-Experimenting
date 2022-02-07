using Godot;
using System;

namespace FaffLatest.scripts.movement
{
    public static class NavigationHelper
    {
        public static MovementPathNode[] GetMovementPathNodes(Vector3[] simplePath)
        {
            var convertedPath = new MovementPathNode[simplePath.Length - 1];

            //GD.Print($"Found path is {string.Join(",", simplePath)}");

            for (var x = 1; x < simplePath.Length; x++)
            {
                convertedPath[x - 1] = InitialiseVariablesForNextTargetDestinationInPath(simplePath[x - 1], simplePath[x], 1);
            }

            return convertedPath;
        }

        public static MovementPathNode[] GetMovementPathNodes(this Navigation navigation, Transform start, Vector3 end)
        {
            var startY = start.origin.y;
            end = new Vector3(end.x, startY, end.z);

            //GD.Print($"Going from {start.origin} to {end}");

            var simplePath = navigation.GetSimplePath(start.origin, end, true);

            //GD.Print($"Simple path is {string.Join(",", simplePath)}");

            var convertedPath = new MovementPathNode[simplePath.Length];

            for (var x = 0; x < simplePath.Length; x++)
            {
                simplePath[x] = new Vector3(simplePath[x].x, startY, simplePath[x].z);

                var startToUse = x == 0 ? start : convertedPath[x - 1].RotationTarget;

                convertedPath[x] = InitialiseVariablesForNextTargetDestinationInPath(startToUse, simplePath[x], startY);
            }

            return convertedPath;
        }


        private static MovementPathNode InitialiseVariablesForNextTargetDestinationInPath(Transform start, Vector3 end, float yToUse)
        {
            //GD.Print($"Calculating from {start} to {end}");

            var destination = new Vector3(end.x, yToUse, end.z);
            var movementVector = CalculateMovementVector(start.origin, end);
            var rotationTarget = start.LookingAt(destination, Vector3.Up);

            //GD.Print($"MV - {movementVector} ");
            //GD.Print($"Destination - {destination}");
            return new MovementPathNode
            {
                Destination = destination,
                MovementVector = movementVector,
                RotationTarget = rotationTarget
            };
        }

        private static MovementPathNode InitialiseVariablesForNextTargetDestinationInPath(Vector3 start, Vector3 end, float yToUse)
        {
            //GD.Print($"Calculating from {start} to {end}");

            var destination = new Vector3(end.x, yToUse, end.z);
            var movementVector = CalculateMovementVector(start, end);

            var startTransform = new Transform(Vector3.Zero, Vector3.Zero, Vector3.Zero, start);

            var rotationTarget = startTransform.LookingAt(destination, Vector3.Up);

            //GD.Print($"MV - {movementVector} ");
            //GD.Print($"Destination - {destination}");
            return new MovementPathNode
            {
                Destination = destination,
                MovementVector = movementVector,
                RotationTarget = rotationTarget
            };
        }

        private static Vector3 CalculateMovementVector(Vector3 start, Vector3 end)
        {
            var movementVector = (end - start).Normalized();
            return movementVector;
        }

    }
}
