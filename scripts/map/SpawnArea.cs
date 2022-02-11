using Godot;
using System;
using System.Collections.Generic;

namespace FaffLatest.scripts.map
{
    public class SpawnableAreas
    {
        public SpawnableAreas()
        {
        }

        public SpawnableAreas(List<Vector3> playerSpawnableAreas, List<Vector3> enemySpawnableAreas)
        {
            PlayerSpawnableAreas = playerSpawnableAreas ?? throw new ArgumentNullException(nameof(playerSpawnableAreas));
            EnemySpawnableAreas = enemySpawnableAreas ?? throw new ArgumentNullException(nameof(enemySpawnableAreas));
        }

        public List<Vector3> PlayerSpawnableAreas { get; set; }

        public List<Vector3> EnemySpawnableAreas { get; set; }
    }
}
