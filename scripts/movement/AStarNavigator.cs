using FaffLatest.scripts.characters;
using FaffLatest.scripts.shared;
using Godot;
using System;
using System.Collections.Generic;

using static FaffLatest.scripts.shared.VectorHelpers;

namespace FaffLatest.scripts.movement
{
    public class AStarNavigator : Node
    {
        public static AStarNavigator Instance;

        [Export]
        public int GridSize { get; private set; } = 1;

        [Export]
        public float YValue { get; private set; } = 0.0f;
        
        private Dictionary<Character, PointInfo> characterLocations;
        private PointInfo[,] points;

        public readonly NonEuclideanAStar AStar = new NonEuclideanAStar();

        public Dictionary<Character, PointInfo> CharacterLocations => characterLocations;
        
        public long Length { get; private set; }
        public long Width { get; private set;  }
        public PointInfo[,] Points { get => points; private set => points = value; }

        public AStarNavigator()
        {
        }

        public override void _Ready()
        {
            base._Ready();
            Instance = this;
        }

        public void AddCharacterLocation(Character character, PointInfo point)
            => characterLocations.Add(character, point);

        public bool TryGetNearestEmptyPointToLocation(Vector3 target, Vector3 origin, out Vector3 point)
            => this.TryGetNearestEmptyPointToLocationWithLoop(target, origin, 5, out point);

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
                AStar.SetPointDisabled(currentPointId);
            }
        }

        private void CreatePoints(int currentPointId, float x, float y, Vector3 location)
        {
            var intX = (int)x;

            var intY = (int)y;

            AStar.AddPoint(currentPointId, location);
            Points[intX, intY] = new PointInfo(currentPointId);
        }

        private void ConnectNearbyNodes(int currentPointId, float x, float z)
        {
            if (x > 0)
            {
                var xAbove = x - GridSize;
                this.GetAndConnectPoints(currentPointId, xAbove, YValue, z);
            }

            if (z > 0)
            {
                var zAbove = z - GridSize;
                this.GetAndConnectPoints(currentPointId, x, YValue, zAbove);
            }
        }

        private void InitialiseComponents(int length, int width)
        {
            var numberOfNodes = length * width;

            AStar.ReserveSpace(numberOfNodes);
            AStar.Clear();

            Points = new PointInfo[length, width];
            characterLocations = new Dictionary<Character, PointInfo>(50);

            Length = length;
            Width = width;
        }

        public GetMovementPathResult TryGetMovementPath(Vector3 start, Vector3 end, Character character) => TryGetMovementPath(start, end, character.ProperBody.MovementStats.AmountLeftToMoveThisTurn);


        public GetMovementPathResult TryGetMovementPath(Vector3 start, Vector3 end, int movementDistance)
        {
            try
            {
                if (OutsideWorldBounds(start, end))
                    return new GetMovementPathResult(false);;

                var (startPoint, endPoint) = this.GetStartAndEndPoints(start, end);

                if (endPoint.OccupyingNode != null)
                    return new GetMovementPathResult(false);

                return this.GetAndCleanPath(start, movementDistance, startPoint, endPoint);
            }
            catch (Exception ex)
            {
                GD.Print($"Error getting path from {start} to {end}- {ex.Message}");
                return new GetMovementPathResult(false);
            }
        }

        private bool OutsideWorldBounds(Vector3 start, Vector3 end)
            => OutsideWorldBounds(start, Width) || OutsideWorldBounds(end, Length);

        private bool OutsideWorldBounds(Vector3 v, float maxValue)
            => v.AnyValueLessThanZero() || v.AnyValueGreaterThanOrEqualToValue(maxValue);
                    
        public void _On_Character_Created(Character character)
        {
            var point = AStar.GetClosestPoint(character.ProperBody.GlobalTransform.origin);
            AStar.SetPointDisabled(point);
            this.CreatePointInfos(character, character.ProperBody);
        }

        public void MarkNodeAsOccupied(Vector3 position)
        {
            var point = AStar.GetClosestPoint(position);

            AStar.SetPointDisabled(point);
        }

        private void _On_Character_FinishedMoving(Character character, Vector3 newPosition)
        {
            if (characterLocations.TryGetValue(character, out PointInfo oldLocationPointInfo))
            {
                AStar.SetPointDisabled(oldLocationPointInfo.Id, false);

                var newOccupyingNode = this.GetPointInfoForLocation(newPosition);
                AStar.SetPointDisabled(newOccupyingNode.Id);

                oldLocationPointInfo.SetOccupier(null);
                newOccupyingNode.SetOccupier(character);

                characterLocations[character] = this.GetPointInfoForLocation(newPosition);
            }
        }

        private void _On_Character_Disposing(Character character)
        {
            if (characterLocations.TryGetValue(character, out PointInfo oldLocationPointInfo))
            {
                AStar.SetPointDisabled(oldLocationPointInfo.Id, false);
                characterLocations.Remove(character);
                GD.Print($"Cleared character from location");
            }
            else
            {
                GD.Print($"Couldn't find chjaracter?");
            }
        }

    }
}