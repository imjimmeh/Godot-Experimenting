using FaffLatest.scripts.constants;
using FaffLatest.scripts.effects.movementguide;
using FaffLatest.scripts.movement;
using Godot;
using System;
using static FaffLatest.scripts.constants.SignalNames;

public class CharacterKinematicBody : KinematicBody
{
	[Signal]
	public delegate void _Character_ClickedOn(Node character, InputEventMouseButton mouseButtonEvent);

	[Signal]
	public delegate void _Character_FinishedMoving(Node character, Vector3 newPosition);

	[Signal]
	public delegate void _Character_ReachedPathPart(Node character, Vector3 part);

	[Signal]
	public delegate void _Character_TurnFinished(Node character);

	public Node Parent;

	[Export]
	public float RotationSpeedInterval { get; set; } = 30.0f;

	[Export]
	public float CurrentRotationSpeed { get; set; } = 0.0f;

	[Export]
	public float MaxRotationSpeed { get; set; } = 0.999f;

	[Export]
	public float MaxSpeed { get; set; } = 2f;

	[Export]
	public Vector3 Velocity { get; set; } =	new Vector3(0, 0, 0);

	[Export]
	public float Acceleration { get; set; } = 0.2f;

	public MovementPathNode[] Path = null;

	public int CurrentPathIndex = 0;

	private bool haveMoreInPath => Path != null && Path.Length > CurrentPathIndex;

	public MovementPathNode CurrentMovementNode => haveMoreInPath ? Path[CurrentPathIndex] : null;

	private bool haveRotated = true;

	public override void _Ready()
	{
		base._Ready();

		if(Parent == null)
		{
			Parent = GetNode<Node>("../");
		}

		Transform = new Transform(Transform.basis, Transform.origin.Round());
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

		if (!haveMoreInPath)
		{
			return;
		}
		
		if(CurrentMovementNode == null)
        {
			GetNextPathPartIfAvailable();
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

	private bool GetNextPathPartIfAvailable()
	{
		//GD.Print("Attempting to get new path part");
		if (!haveMoreInPath)
			return false;

		CurrentPathIndex++;
		
		if (CurrentPathIndex >= Path.GetLength(0))
		{
			//GD.Print("Tried to get new path part but none available");
			ClearPath();
			return false;
		}

		if (CurrentPathIndex > -1)
		{
			try
			{
				EmitSignal(Characters.REACHED_PATH_PART, this, Transform.origin.Round());
			}
			catch
            {
				//GD.Print($"HERE");
				throw;
            }
		}

		return true;
	}

	public void Rotate(float delta)
	{
		if (haveRotated)
			return;

		var (newTransform, newRotationSpeed) = Transform.InterpolateAndRotate(delta, CurrentRotationSpeed, RotationSpeedInterval, MaxRotationSpeed, CurrentMovementNode.RotationTarget);
		Transform = newTransform;
		CurrentRotationSpeed = newRotationSpeed;
		haveRotated = Transform.CurrentRotationMatchesTarget(CurrentMovementNode.MovementVector);
	}

	private void Move(float delta)
	{
		if (!haveMoreInPath)
			return;

		else
		{
			///
			//InterpolateAndMove(delta);
			//GD.Print($"Moving");
			var newVelocity = this.InterpolateAndMove(delta, Velocity, CurrentMovementNode.MovementVector, Acceleration, MaxSpeed);
			Velocity = newVelocity;

			var distance = CurrentMovementNode.Destination.DistanceTo(Transform.origin);
			var atSamePoint = distance <= 0.2f;
			//GD.Print(distance);

			if (atSamePoint)
			{
				if (haveMoreInPath)
				{
					IncrementPath();
				}
				else
				{
					ClearPath();
				}
			}
		}
	}

	private void IncrementPath()
	{
		//GD.Print($"Reached path index {CurrentPathIndex}");
		
		if(!GetNextPathPartIfAvailable())
        {
			return;
        }

		//GD.Print($"Is it null? {Path[CurrentPathIndex] == null}");
		haveRotated = Transform.CurrentRotationMatchesTarget(CurrentMovementNode.MovementVector);

		if (!haveRotated)
		{
			Velocity = new Vector3(CurrentMovementNode.MovementVector.x * 0.2f, 0, CurrentMovementNode.MovementVector.z * 0.2f);
		}

		//GD.Print($"Next path target is {CurrentMovementNode?.Destination} - Movement vector us {CurrentMovementNode?.MovementVector}");
	}

	private void ClearPath()
	{
		//GD.Print($"Reached destination - we are at {Transform.origin}");
		ClearRotation();
		Path = null;
		CurrentPathIndex = -1;
		haveRotated = true;

		var snappedVector = Transform.origin.Round();
		Transform = new Transform(Transform.basis, snappedVector);
	
		//GD.Print($"Snapepd to {snappedVector}");

		EmitSignal(Characters.MOVEMENT_FINISHED, Parent, Transform.origin);
	}

	private void ClearRotation()
	{
		//GD.Print("Rotated towards target");
		CurrentRotationSpeed = 0.0f;
	}

	private void SetInitialMovementVariables()
	{
		Velocity = new Vector3(0, 0, 0);
		haveRotated = false;
		CurrentPathIndex = -1;
		GetNextPathPartIfAvailable();
	}

	private void _On_Character_MoveTo(Node character, MovementPathNode[] path)
	{
		if (character != Parent)
			return;

		//GD.Print($"received new path");

		Path = path;

		//GD.Print($"Path length is {path.Length}");
		SetInitialMovementVariables();

		//GD.Print($"Received movement command - moving to first part in path -  {Path[0].Destination}");
	}

}
