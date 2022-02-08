using FaffLatest.scripts.characters;
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
		private readonly NonEuclideanAStar astar = new NonEuclideanAStar();
		private Dictionary<Node, PointInfo> characterLocations;
		private PointInfo[,] points;

		[Export]
		public int GridSize { get; private set; } = 1;

		[Export]
		public float YValue { get; private set; } = 1.0f;

		public PointInfo[,] Points { get => points; private set => points = value; }

		public long Length { get; private set; }
		public long Width { get; private set; }

		public AStarNavigator()
        {
			CreatePointsForMap(50, 50, new Vector2[0]);
		}

		public override void _Ready()
        {
        }

        public void CreatePointsForMap(int length, int width, Vector2[] initiallyOccupiedPoints)
        {
            InitialiseComponents(length, width);

            int currentPointId = 0;
			float x, y = 0;
            for (x = 0; x < length; x += GridSize)
            {
                for (y = 0; y < width; y += GridSize)
                {
                    CreatePoint(currentPointId, x, y, length, width);

                    currentPointId++;
                }
            }

			//GD.Print($"Created {x} - {y} points");
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
				var yAbove = y -GridSize;
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

        public MovementPathNode[] GetMovementPath(Vector3 start, Vector3 end, float movementDistance)
		{
			try
			{
				(var startX, var startY) = ((int)start.x, (int)start.z);
				(var endX, var endY) = ((int)end.x, (int)end.z);

				PointInfo nearestStart = Points[startX, startY]; // astar.GetClosestPoint(start);
				PointInfo nearestEnd = Points[endX, endY];

				var s = astar.GetPointPosition(nearestStart.Id);
				var e = astar.GetPointPosition(nearestEnd.Id);

				if(nearestEnd.OccupyingNode != null)
                {
					return null;
                }

				//GD.Print($"Nearest Start {s} vs {start} - End is {e} vs {end}");
				var path = astar.GetPointPath(nearestStart.Id, nearestEnd.Id);

				if(path == null)
                {
					//GD.Print($"Could not find path from {nearestStart} to {nearestEnd}");
                }
				var converted = NavigationHelper.GetMovementPathNodes(path, movementDistance);
				//GD.Print($"Going from {start} to {end}");
				//GD.Print($"Start path is {path[0]} and is {path[path.Length - 1]}");

				//GD.Print($"Path count is {path.Length}");
				return converted;
			}
			catch(Exception ex)
			{
				//GD.Print(ex.Message);
				//GD.Print(ex.StackTrace);
				//GD.Print($"Could not get movement path for {start} to {end}");
				throw;
			}
		}

		public void _On_Character_Created(Node character)
        {
            var asCharacter = character as Character;
            //GD.Print($"AStarNavigator received _OnCharacterCreated for character {asCharacter.Stats.CharacterName}");

            var body = asCharacter.CharacterKinematicBody as CharacterKinematicBody;
            var point = astar.GetClosestPoint(body.Transform.origin);

            astar.SetPointDisabled(point);
            CreatePointInfos(character, body);
        }

        private void CreatePointInfos(Node character, CharacterKinematicBody body)
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
			if (characterLocations.TryGetValue(character, out PointInfo oldLocationPointInfo))
			{
				astar.SetPointDisabled(oldLocationPointInfo.Id, false);
				var newOccupyingNode = GetPointInfoForVector3(newPosition);

				astar.SetPointDisabled(newOccupyingNode.Id);

				//GD.Print($"Found new node {newOccupyingNode} for {newPosition} which should be {astar.GetPointPosition(newOccupyingNode.Id)}");

				oldLocationPointInfo.SetOccupier(null);
				newOccupyingNode.SetOccupier(character);

				characterLocations[character] = GetPointInfoForVector3(newPosition);
			}
			else
			{
				//GD.Print($"AStarNavigatorError - Could not find matching node for {character.Name}");
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