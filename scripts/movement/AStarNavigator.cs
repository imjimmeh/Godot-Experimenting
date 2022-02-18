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
        public int GridSize { get; protected internal set; } = 1;

        [Export]
        public float YValue { get; protected internal set; } = 0.0f;
        
        private Dictionary<Character, PointInfo> characterLocations;
        private PointInfo[,] points;

        public readonly NonEuclideanAStar AStar = new NonEuclideanAStar();

        public Dictionary<Character, PointInfo> CharacterLocations 
            { get => characterLocations; protected internal set => characterLocations = value;}
        
        public long Length { get; protected internal set; }
        public long Width { get; protected internal set;  }
        public PointInfo[,] Points { get => points; protected internal set => points = value; }

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


        public GetMovementPathResult TryGetMovementPath(Vector3 start, Vector3 end, Character character) => TryGetMovementPath(start, end, character.Body.MovementStats.AmountLeftToMoveThisTurn);

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
                GD.Print($"Error getting path  {start} to {end}- {ex.Message}");
                return new GetMovementPathResult(false);
            }
        }

        private bool OutsideWorldBounds(Vector3 start, Vector3 end)
            => OutsideWorldBounds(start, Width) || OutsideWorldBounds(end, Length);

        private bool OutsideWorldBounds(Vector3 v, float maxValue)
            => v.AnyValueLessThanZero() || v.AnyValueGreaterThanOrEqualToValue(maxValue);
                    
        public void _On_Character_Created(Character character)
        {
            var point = AStar.GetClosestPoint(character.Body.GlobalTransform.origin);
            AStar.SetPointDisabled(point);
            this.CreatePointInfos(character, character.Body);
        }

        private void _On_Character_FinishedMoving(Character character, Vector3 newPosition)
        {
            if (characterLocations.TryGetValue(character, out PointInfo oldLocationPointInfo))
            {
                this.SetPointFree(oldLocationPointInfo);

                var newOccupyingNode = this.GetPointInfoForLocation(newPosition);
                this.SetPointOccupiedByCharacter(character, newOccupyingNode);

                characterLocations[character] = this.GetPointInfoForLocation(newPosition);
            }
        }
        private void _On_Character_Disposing(Character character)
        {
            if (characterLocations.TryGetValue(character, out PointInfo oldLocationPointInfo))
            {
                AStar.SetPointDisabled(oldLocationPointInfo.Id, false);
                oldLocationPointInfo.SetOccupier(null);
                characterLocations.Remove(character);
            }
        }

    }
}