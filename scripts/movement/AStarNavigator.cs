using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.shared;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.movement
{
    public class AStarNavigator : Node
    {
        public static AStarNavigator Instance;

        private readonly NonEuclideanAStar astar = new NonEuclideanAStar();
        private Dictionary<Character, PointInfo> characterLocations;
        private PointInfo[,] points;

        [Export]
        public int GridSize { get; private set; } = 1;

        [Export]
        public float YValue { get; private set; } = 0.0f;

        public PointInfo[,] Points { get => points; private set => points = value; }

        public long Length { get; private set; }
        public long Width { get; private set; }

        public Dictionary<Character, PointInfo> CharacterLocations => characterLocations;

        public AStarNavigator()
        {
        }

        public override void _Ready()
        {
            base._Ready();
            Instance = this;
        }

        public void CreatePointsForMap(int length, int width, Vector2[] initiallyOccupiedPoints)
        {
            InitialiseComponents(length, width);

            int currentPointId = 0;

            for (var x = 0; x < width; x += GridSize)
            {
                for (var y = 0; y < length; y += GridSize)
                {
                    CreatePoint(currentPointId, x, y, length, width);

                    currentPointId++;
                }
            }
        }

        private void CreatePoint(int currentPointId, float x, float y, int xLength, int yLength)
        {
            var location = new Vector3(x, YValue, y);

            CreatePoints(currentPointId, x, y, location);

            ConnectNearbyNodes(currentPointId, x, y);

            var nodeIsAtEdge = x == 0 || y == 0 || x + 1 == xLength || y + 1 == yLength;

            if (nodeIsAtEdge)
            {
                astar.SetPointDisabled(currentPointId);
            }
        }

        private void CreatePoints(int currentPointId, float x, float y, Vector3 location)
        {
            var intX = (int)x;

            var intY = (int)y;

            astar.AddPoint(currentPointId, location);
            Points[intX, intY] = new PointInfo(currentPointId);
        }

        private void ConnectNearbyNodes(int currentPointId, float x, float y)
        {
            if (x > 0)
            {
                var xAbove = x - GridSize;
                GetAndConnectPoints(currentPointId, xAbove, y);
            }

            if (y > 0)
            {
                var yAbove = y - GridSize;
                GetAndConnectPoints(currentPointId, x, yAbove);
            }
        }

        private void InitialiseComponents(int length, int width)
        {
            var numberOfNodes = length * width;

            astar.ReserveSpace(numberOfNodes);
            astar.Clear();

            Points = new PointInfo[length, width];
            characterLocations = new Dictionary<Character, PointInfo>(50);

            Length = length;
            Width = width;
        }

        public GetMovementPathResult TryGetMovementPath(Vector3 start, Vector3 end, Character character) => TryGetMovementPath(start, end, character.ProperBody.MovementStats.AmountLeftToMoveThisTurn);

        public bool TryGetNearestEmptyPointToLocation(Vector3 target, Vector3 origin, out Vector3 point, int tryCount = 0)
        {
            point = Vector3.Zero;

            if (!IsPointDisabled(target))
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
                    foundPoint = IsLocationValidAndFree(target, tryCount, ref closestDistance, x, y, out point);

                    if (closestDistance <= 1.001f)
                        break;
                }
            }

            return foundPoint;
        }

        private bool IsLocationValidAndFree(Vector3 target, int tryCount, ref float closestDistance, int x, int y, out Vector3 newPoint)
        {
            bool foundPoint = IsValidPositionAndIsFree(tryCount, x, y, target, this, out newPoint);

            if (IsPointCloserToTargetThanCurrent(target, newPoint, closestDistance, out float newDistance))
            {
                closestDistance = newDistance;
            }

            return foundPoint;
        }

        public bool TryGetNearestEmptyPointToLocationWithLoop(Vector3 target, Vector3 origin, out Vector3 point, int maxTryCount)
        {
            int tryCount = -1;
            point = Vector3.Zero;
            bool success;

            while(true)
            {
                tryCount++;
                success = TryGetNearestEmptyPointToLocation(target, origin, out point, tryCount);

                if (success || tryCount == maxTryCount)
                    break;
            }

            return success;
        }

        private static bool IsValidPositionAndIsFree(int tryCount, int x, int y, Vector3 target, AStarNavigator astar, out Vector3 newPoint)
        {
            newPoint = Vector3.Zero;

            if (tryCount > 0 && (x == 0 || y == 0))
                return false;

            newPoint = new Vector3(target.x + x, target.y, target.z + y).Ceil();

            if (newPoint.x < 0 || newPoint.y < 0)
                return false;

            var pointDisabled = astar.IsPointDisabled(newPoint);

            return pointDisabled;
        }

        private static bool IsPointCloserToTargetThanCurrent(Vector3 target, Vector3 newPoint, float currentClosestDistance, out float distance)
        {
            var vectorDistance = (target - newPoint).Abs();
            distance = vectorDistance.x + vectorDistance.z;

            return distance < currentClosestDistance;
        }

        private bool IsPointDisabled(Vector3 target)
        {
            var pointInfo = GetPointInfoForVector3(target);
            var point = astar.GetPointPosition(pointInfo.Id);
            var disabled = astar.IsPointDisabled(pointInfo.Id);

            return disabled;
        }

        public GetMovementPathResult TryGetMovementPath(Vector3 start, Vector3 end, int movementDistance)
        {
            try
            {
                var (startPoint, endPoint) = GetStartAndEndPoints(start, end);

                if (endPoint.OccupyingNode != null)
                    return new GetMovementPathResult(false);

                return GetAndCleanPath(start, movementDistance, startPoint, endPoint);
            }
            catch (Exception ex)
            {
                GD.Print($"Error getting path from {start} to {end}- {ex.Message}");
                return new GetMovementPathResult(false);
            }
        }

        private GetMovementPathResult GetAndCleanPath(Vector3 start, int movementDistance, PointInfo startPoint, PointInfo endPoint)
        {       
            var path = astar.GetPointPath(startPoint.Id, endPoint.Id);

            if (path == null || path.Length < 2)
                return new GetMovementPathResult(false);

            var trimmedPath = TrimAndClampPath(path, start, movementDistance);

            var success = trimmedPath != null && trimmedPath.Length > 0;

            return new GetMovementPathResult(trimmedPath, success);
        }

        private Vector3[] TrimAndClampPath(Vector3[] points, Vector3 start, int maxLength)
        {
            if (points == null || points.Length == 0)
                return null;
            else if (points.Length == 1)
                return points;

            var pointsCount = points.Count() - 1;
            var pathLongerThanAllowed = pointsCount > maxLength;
            var maxX = pathLongerThanAllowed ? maxLength : pointsCount;

            var newArray = new Vector3[maxX];

            for (var x = 1; x <= maxX; x++)
                newArray[x - 1] = points[x];

            return newArray;
        }

        private (PointInfo start, PointInfo end) GetStartAndEndPoints(Vector3 start, Vector3 end)
        {
            CleanStartAndEnd(ref start, ref end);

            (var startX, var startY) = ((int)start.x, (int)start.z);
            (var endX, var endY) = ((int)end.x, (int)end.z);

            return (start: Points[startX, startY], end: Points[endX, endY]);
        }

        private static void CleanStartAndEnd(ref Vector3 start, ref Vector3 end)
        {
            start = start.Ceil();
            end = end.Ceil();
            end = end.CopyYValue(start);
        }

        public void _On_Character_Created(Character character)
        {
            var point = astar.GetClosestPoint(character.ProperBody.GlobalTransform.origin);
            astar.SetPointDisabled(point);
            CreatePointInfos(character, character.ProperBody);
        }

        public void MarkNodeAsOccupied(Vector3 position)
        {
            var point = astar.GetClosestPoint(position);

            astar.SetPointDisabled(point);
        }

        private void CreatePointInfos(Character character, MovingKinematicBody body)
        {
            var pointInfo = GetPointInfoForVector3(body.GlobalTransform.origin);
            pointInfo.SetOccupier(character);
            characterLocations.Add(character, pointInfo);
        }

        private bool GetAndConnectPoints(int currentPoint, float x, float y)
        {
            var otherPoint = astar.GetClosestPoint(new Vector3(x, YValue, y));

            if (otherPoint == currentPoint)
                return false;

            astar.ConnectPoints(otherPoint, currentPoint);
            return true;
        }

        private void _On_Character_FinishedMoving(Character character, Vector3 newPosition)
        {
            if (characterLocations.TryGetValue(character, out PointInfo oldLocationPointInfo))
            {
                astar.SetPointDisabled(oldLocationPointInfo.Id, false);

                var newOccupyingNode = GetPointInfoForVector3(newPosition);
                astar.SetPointDisabled(newOccupyingNode.Id);

                oldLocationPointInfo.SetOccupier(null);
                newOccupyingNode.SetOccupier(character);

                characterLocations[character] = GetPointInfoForVector3(newPosition);
            }
        }

        public PointInfo GetPointInfoForVector3(Vector3 location)
            => GetPointInfoForFloats(location.x, location.z);

        private PointInfo GetPointInfoForFloats(float x, float y)
        {
            var intX = (int)x;

            var intY = (int)y;

            return Points[intX, intY];
        }
    }
}