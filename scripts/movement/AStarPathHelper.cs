using System.Linq;
using Godot;

namespace FaffLatest.scripts.movement{
    public static class AStarPathHelpers
    {   
        public static GetMovementPathResult GetAndCleanPath(this AStarNavigator astar, Vector3 start, int movementDistance, PointInfo startPoint, PointInfo endPoint)
        {       
            var path = astar.AStar.GetPointPath(startPoint.Id, endPoint.Id);

            if (path == null || path.Length < 2)
                return new GetMovementPathResult(false);

            var trimmedPath = TrimPath(path, start, movementDistance);

            return TrimPath(path, start, movementDistance);
        }

        private static GetMovementPathResult TrimPath(Vector3[] points, Vector3 start, int maxLength)
        {   
            if (points == null || points.Length == 0)
                return new GetMovementPathResult(false);
            else if (points.Length == 1)
            {
                return new GetMovementPathResult(points, true, false);
            }

            var pointsCount = points.Length - 1;
            var pathLongerThanAllowed = pointsCount > maxLength;
            var maxX = pathLongerThanAllowed ? maxLength : pointsCount;

            var newArray = new Vector3[maxX];

            for (var x = 1; x <= maxX; x++)
                newArray[x - 1] = points[x];

            return new GetMovementPathResult(newArray, true, pathLongerThanAllowed);
        }

    }
}