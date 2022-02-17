using FaffLatest.scripts.characters;
using Godot;
using System.Collections;
using System.Collections.Generic;

namespace FaffLatest.scripts.movement
{
    public static class AStarFactory
    {
        public static void CreatePointsForMap(this AStarNavigator astar, int length, int width, Vector2[] initiallyOccupiedPoints)
        {
            astar.InitialiseComponents(length, width);

            int currentPointId = 0;

            for (var x = 0; x < width; x += astar.GridSize)
            {
                for (var y = 0; y < length; y += astar.GridSize)
                {
                    var point = astar.CreatePoint(currentPointId, x, y, length, width);

                    astar.Points[x,y] = point;
                    currentPointId++;
                }
            }
        }

        private static void InitialiseComponents(this AStarNavigator astar, int length, int width)
        {
            var numberOfNodes = length * width;

            astar.AStar.ReserveSpace(numberOfNodes);
            astar.AStar.Clear();

            astar.Points = new PointInfo[length, width];
            astar.CharacterLocations = new Dictionary<Character, PointInfo>(50);

            astar.Length = length;
            astar.Width = width;
        }

        private static PointInfo CreatePoint(this AStarNavigator astar, int currentPointId, float x, float y, int xLength, int yLength)
        {
            var location = new Vector3(x, astar.YValue, y);

            var pointInfo = astar.CreatePoints(currentPointId, x, y, location);

            astar.ConnectNearbyNodes(currentPointId, x, y);

            var nodeIsAtEdge = x == 0 || y == 0 || x + 1 == xLength || y + 1 == yLength;

            if (nodeIsAtEdge)
            {
                astar.AStar.SetPointDisabled(currentPointId);
            }

            return pointInfo;
        }


        private static void ConnectNearbyNodes(this AStarNavigator astar, int currentPointId, float x, float z)
        {
            if (x > 0)
            {
                var xAbove = x - astar.GridSize;
                astar.GetAndConnectPoints(currentPointId, xAbove, astar.YValue, z);
            }

            if (z > 0)
            {
                var zAbove = z - astar.GridSize;
                astar.GetAndConnectPoints(currentPointId, x, astar.YValue, zAbove);
            }
        }

    
        private static PointInfo CreatePoints(this AStarNavigator astar, int currentPointId, float x, float y, Vector3 location)
        {
            var intX = (int)x;

            var intY = (int)y;

            astar.AStar.AddPoint(currentPointId, location);
            return new PointInfo(currentPointId);
        }
    }
}