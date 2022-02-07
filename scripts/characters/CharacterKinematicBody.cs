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

	public Node Parent;

	[Export]
	public float RotationSpeedInterval = 30.0f;

	[Export]
	public float CurrentRotationSpeed = 0.0f;

	[Export]
	public float MaxRotationSpeed = 0.999f;

	[Export]
	public float MaxSpeed = 2f;

	[Export]
	public Vector3 Velocity = new Vector3(0, 0, 0);

	[Export]
	public float Acceleration = 0.2f;

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
	}

	public override void _InputEvent(Godot.Object camera, InputEvent inputEvent, Vector3 position, Vector3 normal, int shapeIdx)
	{
		if (inputEvent is InputEventMouseButton mouseButtonEvent)
		{
			GD.Print("Character clicked on - emitting signal");
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
		else if(CurrentMovementNode == null)
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
		GD.Print("Attempting to get new path part");
		if (!haveMoreInPath)
			return false;

		CurrentPathIndex++;
		
		if (CurrentPathIndex >= Path.GetLength(0))
		{
			GD.Print("Tried to get new path part but none available");
			ClearPath();
			return false;
		}

		return true;
	}

	public void Rotate(float delta)
	{
		if (haveRotated)
			return;

		if (CurrentRotationMatchesCurrentRotationTarget())
		{
			haveRotated = true;
		}
		else
		{
			InterpolateAndRotate(delta);
		}
	}

	private void Move(float delta)
	{
		if (!haveMoreInPath)
			return;

		var distance = CurrentMovementNode.Destination.DistanceTo(Transform.origin);
		var atSamePoint = distance <= 0.5f;
		
		if(atSamePoint)
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
		else
		{
			//GD.Print(distance);
			InterpolateAndMove(delta);
		}
	}

	private void IncrementPath()
	{
		GD.Print($"Reached path index {CurrentPathIndex}");
		
		if(!GetNextPathPartIfAvailable())
        {
			return;
        }

		GD.Print($"Is it null? {Path[CurrentPathIndex] == null}");
		haveRotated = CurrentRotationMatchesCurrentRotationTarget();

		if (!haveRotated)
		{
			Velocity = new Vector3(CurrentMovementNode.MovementVector.x * 0.2f, 0, CurrentMovementNode.MovementVector.z * 0.2f);
		}

		GD.Print($"Next path target is {CurrentMovementNode?.Destination} - Movement vector us {CurrentMovementNode?.MovementVector}");
	}

	private void ClearPath()
	{
		GD.Print($"Reached destination - we are at {Transform.origin}");
		ClearRotation();
		Path = null;
		CurrentPathIndex = -1;
		haveRotated = true;

		var snappedVector = Transform.origin.Round();
		Transform = new Transform(Transform.basis, snappedVector);
		GD.Print($"Snapepd to {snappedVector}");
		EmitSignal("_Character_FinishedMoving", Parent, Transform.origin);

	}

	private void InterpolateAndMove(float delta)
	{
		var newVelocity = Velocity + CurrentMovementNode.MovementVector * Acceleration * delta;

		if (newVelocity.Length() < MaxSpeed)
		{
			Velocity = newVelocity;
		}

		var collision = MoveAndCollide(Velocity);

		//if(collision == null)
		//{
		//	MoveAndCollide(Velocity);
		//}
  //      else
  //      {
		//	if (!haveMoreInPath)
		//	{
		//		GD.Print($"Collided and path fin - clearing");
		//		ClearPath();
		//	}
		//	else
		//	{
		//		GD.Print($"Collided and not fin - moving on");
		//		GetNextPathPartIfAvailable();
		//	}
		//}
	}

	private void InterpolateAndRotate(float delta)
	{
		CurrentRotationSpeed = Mathf.SmoothStep(CurrentRotationSpeed, 1.0f, RotationSpeedInterval * delta);
		CurrentRotationSpeed = Mathf.Clamp(CurrentRotationSpeed, 0.0f, 1.0f);

		if (!haveRotated)
		{
			try
			{
				Transform = Transform.InterpolateWith(CurrentMovementNode.RotationTarget, CurrentRotationSpeed);
			}
			catch (Exception ex)
			{
				Path[CurrentPathIndex] = new MovementPathNode
				{
					Destination = CurrentMovementNode.Destination,
					MovementVector = CurrentMovementNode.MovementVector,
					RotationTarget = Transform.LookingAt(CurrentMovementNode.Destination, new Vector3(0.0f, 100.0f, 0.0f))
				};

				GD.Print(ex.Message + " - " + CurrentRotationSpeed + " - " + CurrentMovementNode.RotationTarget.origin + " - " + Transform.origin + " - " + CurrentMovementNode.RotationTarget.basis);
			}
		}
	}

	private void ClearRotation()
	{
		GD.Print("Rotated towards target");
		CurrentRotationSpeed = 0.0f;
	}

	private bool CurrentRotationMatchesCurrentRotationTarget()
	{
		var destinationVector = Transform.basis.z.Normalized();

		var dotProduct = CurrentMovementNode.MovementVector.Dot(destinationVector);

		var lookingAtDestination = dotProduct <= -0.9999f;

		//GD.Print(dotProduct);
		return lookingAtDestination;
	}

	private bool CurrentRotationMatchesTargetRotation(Vector3 backwardsVector, Vector3 movementVector)
	{
		var dotProduct = movementVector.Dot(backwardsVector);

		var lookingAtDestination = dotProduct < -0.9999f;
		return lookingAtDestination;
	}


	private void SetInitialMovementVariables()
	{
		Velocity = new Vector3(0, 0, 0);
		haveRotated = false;
		CurrentPathIndex = -1;
		GetNextPathPartIfAvailable();
	}

	private void _On_MoveTo(Node character, MovementPathNode[] path)
	{
		if (character != Parent)
			return;

		GD.Print($"received new path");

		Path = path;

		GD.Print($"Path length is {path.Length}");
		SetInitialMovementVariables();

		GD.Print($"Moving to next part in path -  {Path[0].Destination}");
	}

}
