using FaffLatest.scripts.movement;
using Godot;

public interface IMovingKinematicBody
{
    Vector3? CurrentCurrentMovementDestination { get; set; }

    void SetTarget(Vector3? target);
}