using FaffLatest.scripts.constants;
using FaffLatest.scripts.effects.movementguide;
using FaffLatest.scripts.movement;
using Godot;
using System;
using static FaffLatest.scripts.constants.SignalNames;

public class MovingKinematicBody : KinematicBody
{
    private const float DISTANCE_MARGIN_ALLOWED = 0.15f;

    [Signal]
    public delegate void _Character_ClickedOn(Node character, InputEventMouseButton mouseButtonEvent);

    [Signal]
    public delegate void _Character_FinishedMoving(Node character, Vector3 newPosition);

    [Signal]
    public delegate void _Character_TurnFinished(Node character);

    [Export]
    public MovementStats MovementStats { get; private set; }


    private bool haveRotated = false;
    private PathMover pathMover;

    public Node Parent;

    public Vector3 CurrentMovementDestination { get; private set; }
    public Vector3 CurrentMovementVector { get; private set; }
    public Transform TargetRotation { get; private set; }


    public bool HaveDestination = false;

    public override void _Ready()
    {
        base._Ready();

        if (Parent == null)
        {
            Parent = GetNode<Node>("../");
        }

        Transform = new Transform(Transform.basis, Transform.origin.Round());
        pathMover = GetNode<PathMover>("PathMover");
    }

    public override void _InputEvent(Godot.Object camera, InputEvent inputEvent, Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (inputEvent is InputEventMouseButton mouseButtonEvent)
        {
            //GD.Print("Character clicked on - emitting signal");
            EmitSignal(Characters.CLICKED_ON, Parent, mouseButtonEvent);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (!HaveDestination)
        {
            if (!GetNextDestination())
            {
                return;
            }

        }

        if (!haveRotated)
        {
            Rotate(delta);
        }
        else
        {
            Move(delta);
        }
    }

    private void Rotate(float delta)
    {
        if (!HaveDestination)
            return;

        if (HaveRotated())
            return;

        if (TargetRotation == null)
        {
            TargetRotation = Transform.LookingAt(CurrentMovementDestination, Vector3.Up);
        }

        this.CalculateRotation(delta);

        Transform = this.InterpolateRotation(CurrentMovementDestination);

    }

    private bool HaveRotated()
    {
        if (CurrentMovementVector == Vector3.Zero || CurrentMovementDestination == null)
            return true;

        haveRotated = Transform.CurrentRotationMatchesTarget(CurrentMovementVector);

        if (haveRotated)
        {
            Vector3 direction = (CurrentMovementDestination - Transform.origin).Normalized();

            var dotProduct = MovementStats.Velocity.Normalized().Dot(direction);
            var sameDirection = dotProduct == 1f;

            if (!sameDirection)
            {
                MovementStats.SetVelocity(Vector3.Zero);
            }

            SnapRotation();
        }


        return haveRotated;
    }

    private void SnapRotation()
    {
        Transform = Transform.LookingAt(CurrentMovementDestination, Vector3.Up);
    }

    private void Move(float delta)
    {
        if (!HaveDestination)
            return;

        if (MovementStats.AmountLeftToMoveThisTurn <= 0)
        {
            GD.Print("Can't move anymore this turn");
            MovementFinished();
            pathMover.ClearPath();
            return;
        }

        this.InterpolateAndMove(delta, CurrentMovementDestination);

        bool atSamePoint = ReachedCurrentDestination();
        //GD.Print(distance);

        if (atSamePoint)
        {
            GetNextDestination();
        }
    }

    private bool GetNextDestination()
    {
        if (!pathMover.HaveMoreInPath)
        {
            return false;
        }
        else
        {
            if (CurrentMovementDestination != Vector3.Zero)
                SnapToCurrentDestination();

            if (pathMover.TryGetNextPathPart(out Vector3 target))
            {
                SetVariables(target);
                return true;
            }
            else
            {
                ResetVariables();
                return false;
            }
        }
    }

    private void MovementFinished()
    {
        ResetVariables();
        EmitSignal(Characters.MOVEMENT_FINISHED, Parent, Transform.origin);
    }

    private void SnapToCurrentDestination()
    {
        Transform = new Transform(Transform.basis, CurrentMovementDestination);
    }

    private void ResetVariables()
    {
        CurrentMovementDestination = Vector3.Zero;
        HaveDestination = false;
        haveRotated = false;
        CurrentMovementVector = Vector3.Zero;

        MovementStats.SetVelocity(Vector3.Zero);
        MovementStats.SetCurrentRotationSpeed(0f);
    }

    private void SetVariables(Vector3 target)
    {
        CurrentMovementDestination = target;
        HaveDestination = true;

        var newTargetMovementVector = (target - Transform.origin).Normalized();

        CurrentMovementVector = newTargetMovementVector;
        haveRotated = HaveRotated();
    }

    private bool ReachedCurrentDestination()
    {
        var withSameYOrigin = new Vector3(Transform.origin.x, CurrentMovementDestination.y, Transform.origin.z);
        var distance = CurrentMovementDestination.DistanceTo(withSameYOrigin);
        var atSamePoint = distance <= DISTANCE_MARGIN_ALLOWED;


        return atSamePoint;
    }
}