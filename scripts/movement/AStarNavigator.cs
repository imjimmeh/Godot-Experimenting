using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
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
		public const string GLOBAL_SCENE_PATH = NodeReferences.Systems.ASTAR;

		private readonly NonEuclideanAStar astar = new NonEuclideanAStar();
		private Dictionary<Node, PointInfo> characterLocations;
		private PointInfo[,] points;

		[Export]
		public int GridSize { get; private set; } = 1;

		[Export]
		public float YValue { get; private set; } = 0.0f;

		public PointInfo[,] Points { get => points; private set => points = value; }

		public long Length { get; private set; }
		public long Width { get; private set; }

		public Dictionary<Node, PointInfo> CharacterLocations => characterLocations;

		public AStarNavigator()
        {
		}

		public override void _Ready()
        {
        }

        public void CreatePointsForMap(int length, int width, Vector2[] initiallyOccupiedPoints)
        {
            InitialiseComponents(length, width);

            int currentPointId = 0;

            for (var x = 0; x < length; x += GridSize)
            {
                for (var y = 0; y < width; y += GridSize)
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

			if(nodeIsAtEdge)
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
			characterLocations = new Dictionary<Node, PointInfo>(50);

			Length = length;
			Width = width;
        }

		public Vector3[] GetMovementPath(Vector3 start, Vector3 end, int movementDistance)
		{
			try
            {
                var(startPoint, endPoint) = GetStartAndEndPoints(start, end);

                var s = astar.GetPointPosition(startPoint.Id);
                var e = astar.GetPointPosition(endPoint.Id);

                if (endPoint.OccupyingNode != null)
                {
                    return null;
                }

                var path = astar.GetPointPath(startPoint.Id, endPoint.Id);

                if (path == null)
                {
                    GD.Print($"Could not find path for {start} to {end}");

                    return null;
                }

                GD.Print($"found path {string.Join(",", path)}");
                return TrimAndClampPath(path, start, movementDistance);
            }
            catch (Exception ex)
			{

				GD.Print($"Error getting path from {start} to {end}- {ex.Message}");
				return null;
			}
		}

        private Vector3[] TrimAndClampPath(Vector3[] points, Vector3 start, int maxLength)
        {
            if(points == null || points.Length == 0)
            {
                throw new Exception("No path given");
            }
            else if(points.Length == 1)
            {
                throw new Exception("Only one point on path given");
            }
            else if(start.z != points[0].z || start.z != points[0].z)
            {
                GD.Print($"Already missing start point - initial is {points[0]}, start vector given was {start}");
                return points;
            }

            var pointsCount = points.Count() - 1;

            var pathLongerThanAllowed = pointsCount > maxLength;
            var maxX = pathLongerThanAllowed ? maxLength + 1 : pointsCount;

            var newArray = new Vector3[maxX];

            for(var x = 0; x <= maxX; x++)
            {
                if (x == 0)
                    continue;

                newArray[x - 1] = points[x];
            }

            return newArray;
        }

        private (PointInfo start, PointInfo end) GetStartAndEndPoints(Vector3 start, Vector3 end)
        {
            (var startX, var startY) = ((int)start.x, (int)start.z);
            (var endX, var endY) = ((int)end.x, (int)end.z);

            return (start: Points[startX, startY], end: Points[endX, endY]);
        }

        public void _On_Character_Created(Node character)
        {
            var asCharacter = character as Character;
            //GD.Print($"AStarNavigator received _OnCharacterCreated for character {asCharacter.Stats.CharacterName}");

            var body = asCharacter.Body as MovingKinematicBody;
            var point = astar.GetClosestPoint(body.Transform.origin);

            astar.SetPointDisabled(point);
            CreatePointInfos(character, body);
        }

        private void CreatePointInfos(Node character, MovingKinematicBody body)
        {	
            var pointInfo = GetPointInfoForVector3(body.Transform.origin);
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

		private void _On_Character_FinishedMoving(Node character, Vector3 newPosition)
		{
			GD.Print($"Char finished moving - {newPosition}");
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