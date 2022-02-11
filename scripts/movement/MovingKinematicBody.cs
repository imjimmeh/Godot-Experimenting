using FaffLatest.scripts.constants;
using FaffLatest.scripts.effects.movementguide;
using FaffLatest.scripts.movement;
using Godot;
using System;
using static FaffLatest.scripts.constants.SignalNames;

public class MovingKinematicBody : KinematicBody
{
    [Signal]
    public delegate void _Character_ClickedOn(Node character, InputEventMouseButton mouseButtonEvent);

    [Signal]
    public delegate void _Character_FinishedMoving(Node character, Vector3 newPosition);

    [Signal]
    public delegate void _Character_TurnFinished(Node character);

    [Export]
    public MovementStats MovementStats { get; private set; }


    public Node Parent;

    public Vector3 CurrentMovementDestination;
    public Vector3 CurrentMovementVector;
    public Transform TargetRotation;

    public bool HaveDestination = false;

    private bool haveRotated = false;
    private PathMover pathMover;

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
            if(!GetNextDestination())
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

        if(TargetRotation == null)
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
            MovementStats.SetVelocity(Vector3.Zero);
        }


        return haveRotated;
    }

    private void Move(float delta)
    {
        if (!HaveDestination)
            return;

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
            pathMover.ClearPath();
            return false;
        }
        else
        {
            if (pathMover.TryGetNextPathPart(out Vector3 target))
            {
                GD.Print($"Got new target destination of {target}");
                CurrentMovementDestination = target;
                HaveDestination = true;
                
                var newTargetMovementVector = (target - Transform.origin).Normalized();

                haveRotated = CurrentMovementVector.IsEqualApprox(newTargetMovementVector);
                CurrentMovementVector = newTargetMovementVector;
                GD.Print($"new target is on same rotation is {haveRotated}");

                return true;
            }
            else
            {
                CurrentMovementDestination = Vector3.Zero;
                HaveDestination = false;
                haveRotated = false;
                CurrentMovementVector = Vector3.Zero;

                return false;
            }
        }
    }

    private bool ReachedCurrentDestination()
    {
        var withSameYOrigin = new Vector3(Transform.origin.x, CurrentMovementDestination.y, Transform.origin.z);
        var distance = CurrentMovementDestination.DistanceTo(withSameYOrigin);
        var atSamePoint = distance <= 0.05f;

        return atSamePoint;
    }

    private void SetInitialMovementVariables()
    {
        haveRotated = false;
        MovementStats.SetVelocity(Vector3.Zero);
        MovementStats.SetCurrentRotationSpeed(0.0f);
    }
}
