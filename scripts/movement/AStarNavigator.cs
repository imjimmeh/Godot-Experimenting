using FaffLatest.scripts.characters;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.movement
{
	public class AStarNavigator : Godot.Node
	{
		[Export]
		public int GridSize = 1;

		[Export]
		public float YValue = 1.0f;

		public Dictionary<(float, float), int> PointIds;

		private AStar astar = new AStar();

		public Dictionary<Node, int> OccupyingNodes = new Dictionary<Node, int>(100);

		public AStarNavigator()
        {
			CreatePointsForMap(50, 50, new Vector2[0]);
		}

		public override void _Ready()
		{
		}

		public void CreatePointsForMap(int length, int width, Vector2[] initiallyOccupiedPoints)
		{
			astar.Clear();

			int currentPoint = 0;

			PointIds = new Dictionary<(float, float), int>(length * width);

			for (float x = 0; x < length; x += GridSize)
			{
				for (float y = 0; y < width; y += GridSize)
				{
					var location = new Vector3(x, YValue, y);

					astar.AddPoint(currentPoint, location);
					PointIds.Add((x, y), currentPoint);

					if (x > 0)
					{
						GetAndConnectPoints(currentPoint, x - GridSize, y);
					}

					if (y > 0)
					{
						GetAndConnectPoints(currentPoint, x, y - GridSize);
					}

					currentPoint++;
				}
			}
		}

		public MovementPathNode[] GetMovementPath(Vector3 start, Vector3 end)
		{
			try
			{
				var nearestStart = astar.GetClosestPoint(start);
				GD.Print($"Start is {start} vs nearest start {nearestStart} which is {astar.GetPointPosition(nearestStart)}");

				var nearestEnd = astar.GetClosestPoint(end);
				GD.Print($"end is {end} vs nearest end {nearestEnd}");

				var path = astar.GetPointPath(nearestStart, nearestEnd);

				var converted = NavigationHelper.GetMovementPathNodes(path);

				return converted;
			}
			catch
			{
				GD.Print($"Could not get movement path for {start} to {end}");
				throw;
			}
		}

		public void _On_Character_Created(Node character)
		{
			var asCharacter = character as Character;
			GD.Print(asCharacter.CharacterKinematicBody == null);
			var body = asCharacter.CharacterKinematicBody as CharacterKinematicBody;

			GD.Print("Charracter being added to astar");
			
			var point = astar.GetClosestPoint(body.Transform.origin);

			GD.Print(astar.GetPointPosition(point));
			astar.SetPointDisabled(point);

			GD.Print($"Disabled point {point}");

			OccupyingNodes.Add(character, point);
		}

		private void GetAndConnectPoints(int currentPoint, float x, float y)
		{
			var pointAbove = PointIds[(x, y)];
			astar.ConnectPoints(pointAbove, currentPoint);
		}

		private void _On_Character_FinishedMoving(Node character, Vector3 newPosition)
		{
			if (OccupyingNodes.TryGetValue(character, out int point))
			{
				astar.SetPointDisabled(point, false);

				var newOccupyingNode = astar.GetClosestPoint(newPosition);
				astar.SetPointDisabled(newOccupyingNode);
				OccupyingNodes[character] = newOccupyingNode;
				GD.Print($"Moved {character.Name} from {point} to {newOccupyingNode}");
			}
			else
			{
				GD.Print($"ERROR - Could not find matching node for {character.Name}");
			}
		}
	}
}