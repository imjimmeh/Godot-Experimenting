using FaffLatest.scripts.state;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.map
{
    public static class MapHelpers
    {
        private const string SPAWNABLE_AREA_SUFFIX = "SpawnableArea";

        private const string ENEMY_SPAWNABLE_AREA = "enemy" + SPAWNABLE_AREA_SUFFIX;
        private const string PLAYER_SPAWNABLE_AREA = "player" + SPAWNABLE_AREA_SUFFIX;

        public static SpawnableAreas GetSpawnArea(this Spatial parent)
        {
            var playerSpawns = parent.GetSpawnAreas(Faction.PLAYER).ToList();
            var enemySpawns = parent.GetSpawnAreas(Faction.ENEMY).ToList();

            return new SpawnableAreas(playerSpawns, enemySpawns);
        }

        public static IEnumerable<Vector3> GetSpawnAreas(this Node n, Faction faction)
        {
            var group = GetSpawnableAreGroupaNameForFaction(faction);
            
            var nodes = n.GetTree().GetNodesInGroup(group);


            foreach (var node in nodes)
            {
                if (node is MeshInstance meshInstance && meshInstance.Mesh is PlaneMesh plane)
                {
                    foreach (var pos in GetSpawnPositionsForMesh(meshInstance, plane))
                        yield return pos;
                }
            }           
        }

        private static IEnumerable<Vector3> GetSpawnPositionsForMesh(MeshInstance mesh, PlaneMesh plane)
        {
            mesh.Hide();
            mesh.QueueFree();

            var size = plane.Size;
            var position = mesh.GlobalTransform.origin;

            float halfX, halfZ, negativeHalfX, negativeHalfZ;
            GetBoundsOfMesh(size, out halfX, out halfZ, out negativeHalfX, out negativeHalfZ);

            for (var x = negativeHalfX; x <= halfX; x++)
            {
                for (var z = negativeHalfZ; z <= halfZ; z++)
                {
                    yield return new Vector3(
                                    position.x + x,
                                    position.y,
                                    position.z + z);
                }
            }
        }

        private static void GetBoundsOfMesh(Vector2 size, out float halfX, out float halfZ, out float negativeHalfX, out float negativeHalfZ)
        {
            (halfX, halfZ) = (size.x / 2, size.y / 2);

            (negativeHalfX, negativeHalfZ) = (halfX * -1, halfZ * -1);
        }

        private static string GetSpawnableAreGroupaNameForFaction(Faction faction) => faction == Faction.ENEMY ? ENEMY_SPAWNABLE_AREA : PLAYER_SPAWNABLE_AREA;
    }
}
