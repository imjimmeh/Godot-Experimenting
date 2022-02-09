using Godot;
using System;

namespace FaffLatest.scripts.characters
{
    public struct CharacterCreationStats
    {
        public Vector3 StartPosition;
        public string Name;
        public int Health;
        public bool IsPlayerCharacter;
        public int MovementDistance;

        public CharacterCreationStats(Vector3 startPosition, string name = null, int health = 0, bool isPlayerCharacter = false, int movementDistance = 0)
        {
            StartPosition = startPosition;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Health = health;
            IsPlayerCharacter = isPlayerCharacter;
            MovementDistance = movementDistance;
        }
    }

}
