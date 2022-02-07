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
		private Dictionary<Node, PointInfo> characterLocations;

		[Export]
		public int GridSize = 1;

		[Export]
		public float YValue = 1.0f;

		private NonEuclideanAStar astar = new NonEuclideanAStar();

		public PointInfo[,] Points;

		
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


            for (float x = 0; x < length; x += GridSize)
            {
                for (float y = 0; y < width; y += GridSize)
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
        }

        public MovementPathNode[] GetMovementPath(Vector3 start, Vector3 end)
		{
			try
			{
				var nearestStart = astar.GetClosestPoint(start);
				//GD.Print($"Start is {start} vs nearest start {nearestStart} which is {astar.GetPointPosition(nearestStart)}");

				var nearestEnd = astar.GetClosestPoint(end);
				//GD.Print($"end is {end} vs nearest end {nearestEnd}");

				var path = astar.GetPointPath(nearestStart, nearestEnd);

				var converted = NavigationHelper.GetMovementPathNodes(path);

				return converted;
			}
			catch
			{
				//GD.Print($"Could not get movement path for {start} to {end}");
				throw;
			}
		}

		public void _On_Character_Created(Node character)
        {
            var asCharacter = character as Character;
            //GD.Print($"AStarNavigator received _OnCharacterCreated for character {asCharacter.Stats.CharacterName}");

            var body = asCharacter.CharacterKinematicBody as CharacterKinematicBody;

            //GD.Print("Charracter being added to astar");
            var point = astar.GetClosestPoint(body.Transform.origin);

            //GD.Print(astar.GetPointPosition(point));
            astar.SetPointDisabled(point);

            //GD.Print($"Disabled point {point}");

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

				var newOccupyingNode = astar.GetClosestPoint(newPosition);
				astar.SetPointDisabled(newOccupyingNode);

				oldLocationPointInfo.SetOccupier(null);

				characterLocations[character] = GetPointInfoForVector3(newPosition);
				//GD.Print($"Moved {character.Name} from {oldLocationPointInfo} to {newOccupyingNode}");
			}
			else
			{
				//GD.Print($"ERROR - Could not find matching node for {character.Name}");
			}
		}

		private PointInfo GetPointInfoForVector3(Vector3 location)
			=> GetPointInfoForFloats(location.x, location.z);

		private PointInfo GetPointInfoForFloats(float x, float y)
		{
			var intX = (int)x;

			var intY = (int)y;

			return Points[intX, intY];
		}
	}
}