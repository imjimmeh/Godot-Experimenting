using FaffLatest.scripts.characters;
using FaffLatest.scripts.shared;
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

        public Vector3 GetSpawnPosition(CharacterStats character)
        {
            List<Vector3> positions = GetPossiblePositionsToSpawnCharacter(character);

            var posToGet = RandomHelper.RNG.RandiRange(0, positions.Count - 1);

            var position = positions[posToGet];

            if (position != null)
            {
                positions.Remove(position);
                position = position.WithValues(y: 0.5f);
                return position;
            }

            return GetSpawnPosition(character);
        }

        public List<Vector3> GetPossiblePositionsToSpawnCharacter(CharacterStats character)
            => character.IsPlayerCharacter ? PlayerSpawnableAreas : EnemySpawnableAreas;
    }
}
