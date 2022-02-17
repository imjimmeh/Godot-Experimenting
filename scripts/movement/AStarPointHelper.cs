using FaffLatest.scripts.characters;
using FaffLatest.scripts.shared;
using Godot;

namespace FaffLatest.scripts.movement
{
    public static class AStarPointHelper
    {
        public static bool IsValidPositionAndIsFree(this AStarNavigator astar, int tryCount, int x, int y, Vector3 target, out Vector3 newPoint)
        {
            newPoint = Vector3.Zero;

            if (tryCount > 0 && (x == 0 || y == 0))
                return false;

            newPoint = new Vector3(target.x + x, target.y, target.z + y).Ceil();

            if (newPoint.x < 0 || newPoint.y < 0)
                return false;

            var pointDisabled = AStarNavigator.Instance.IsPointDisabled(newPoint);

            return pointDisabled;
        }

        public static bool IsPointCloserToTargetThanCurrent(Vector3 target, Vector3 newPoint, float currentClosestDistance, out float distance)
        {
            var vectorDistance = (target - newPoint).Abs();
            distance = vectorDistance.x + vectorDistance.z;

            return distance < currentClosestDistance;
        }

        public static bool IsPointDisabled(this AStarNavigator astar, Vector3 target)
        {
            var pointInfo = astar.GetPointInfoForLocation(target);
            var point = astar.AStar.GetPointPosition(pointInfo.Id);
            var disabled = astar.AStar.IsPointDisabled(pointInfo.Id);

            return disabled;
        }

        
        private static bool TryGetNearestEmptyPointToLocation(this AStarNavigator astar, Vector3 target, Vector3 origin, out Vector3 point, int tryCount = 0)
        {
            point = Vector3.Zero;

            if (!astar.IsPointDisabled(target))
            {
                point = target;
                return true;
            }

            var end = tryCount + 1;
            var start = end * -1;

            float closestDistance = 99999;
            bool foundPoint = false;

            for (int x = start; x <= end; x += end)
            {
                for (int y = start; y <= end; y += end)
                {
                    foundPoint = astar.IsLocationValidAndFree(target, tryCount, ref closestDistance, x, y, out point);

                    if (foundPoint && closestDistance <= 1.001f)
                        break;
                }
            }

            return foundPoint;
        }

        public static bool IsLocationValidAndFree(this AStarNavigator astar, Vector3 target, int tryCount, ref float closestDistance, int x, int y, out Vector3 newPoint)
        {
            bool foundPoint = astar.IsValidPositionAndIsFree(tryCount, x, y, target, out newPoint);

            if (IsPointCloserToTargetThanCurrent(target, newPoint, closestDistance, out float newDistance))
            {
                closestDistance = newDistance;
            }

            return foundPoint;
        }

        public static bool TryGetNearestEmptyPointToLocationWithLoop(this AStarNavigator astar, Vector3 target, Vector3 origin, int maxTryCount, out Vector3 point)
        {
            int tryCount = -1;
            point = Vector3.Zero;
            bool success;

            while(true)
            {
                tryCount++;
                success = astar.TryGetNearestEmptyPointToLocation(target, origin, out point, tryCount);

                if (success || tryCount == maxTryCount)
                    break;
            }

            return success;
        }

        
        public static PointInfo GetPointInfoForLocation(this AStarNavigator astar, Vector3 location)
            => astar.GetPointInfoForLocation(location.x, location.z);

        public static PointInfo GetPointInfoForLocation(this AStarNavigator astar, float x, float y)
        {
            var intX = (int)x;

            var intY = (int)y;

            return astar.Points[intX, intY];
        }

        
        public static void CreatePointInfos(this AStarNavigator astar, Character character, MovingKinematicBody body)
        {
            var pointInfo = astar.GetPointInfoForLocation(body.GlobalTransform.origin);
            pointInfo.SetOccupier(character);
            astar.AddCharacterLocation(character, pointInfo);
        }

        public static bool GetAndConnectPoints(this AStarNavigator astar, int currentPoint, float x, float y, float z)
        {
            var otherPoint = astar.AStar.GetClosestPoint(new Vector3(x, y, z));

            if (otherPoint == currentPoint)
                return false;

            astar.AStar.ConnectPoints(otherPoint, currentPoint);
            return true;
        }

        
        public static (PointInfo start, PointInfo end) GetStartAndEndPoints(this AStarNavigator astar, Vector3 start, Vector3 end)
        {
            CleanStartAndEnd(ref start, ref end);

            (var startX, var startY) = ((int)start.x, (int)start.z);
            (var endX, var endY) = ((int)end.x, (int)end.z);

            return (start: astar.Points[startX, startY], end: astar.Points[endX, endY]);
        }


        private static void CleanStartAndEnd(ref Vector3 start, ref Vector3 end)
        {
            start = start.Ceil();
            end = end.Ceil();
            end = end.CopyYValue(start);
        }
    }
}