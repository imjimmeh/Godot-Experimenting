using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.effects.movementguide
{
    public interface ICharacterMovementGuide
    {
        AStarNavigator AStar { get; }
        PackedScene MovementGuideCellScene { get; }

        void AddCellToArray(CharacterMovementGuideCell cell, float a, float b);
    }
}