using Godot;
using System;
using System.Text;

namespace FaffLatest.scripts.movement
{
    public static class NavigationHelper
    {
        public static MovementPathNode[] GetMovementPathNodes(Vector3[] simplePath, float maxDistance)
        {
            var convertedPath = new MovementPathNode[simplePath.Length - 1];

            //GD.Print($"Found path is {string.Join(",", simplePath)}");
            var sb = new StringBuilder();
            float currentDistance = 0;
            for (var x = 1; x < simplePath.Length; x++)
            {
                convertedPath[x - 1] = InitialiseVariablesForNextTargetDestinationInPath(simplePath[x - 1], simplePath[x], 1);
                currentDistance += convertedPath[x - 1].MovementVector.Length();

                sb.AppendLine(convertedPath[x - 1].Destination.ToString());

                if (currentDistance == maxDistance)
                {
                    return convertedPath;
                }
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
