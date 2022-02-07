using FaffLatest.scripts.constants;
using FaffLatest.scripts.input;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;

namespace FaffLatest.scripts.characters
{

    public struct CharacterCreationStats
    {
        public Vector3 StartPosition;
        public string Name;
        public int Health;
        public bool IsPlayerCharacter;
        public float MovementDistance;

        public CharacterCreationStats(Vector3 startPosition, string name, int health, bool isPlayerCharacter, float movementDistance)
        {
            StartPosition = startPosition;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Health = health;
            IsPlayerCharacter = isPlayerCharacter;
            MovementDistance = movementDistance;
        }
    }

}
