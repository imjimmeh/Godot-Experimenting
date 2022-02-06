using FaffLatest.scripts.characters;
using Godot;
using System;
using static FaffLatest.scripts.constants.SignalNames;

public class CharacterKinematicBody : KinematicBody
{
	[Signal]
	public delegate void _Character_ClickedOn(Node character, InputEventMouseButton mouseButtonEvent);

	public Node Parent;

	public Transform? RotationTarget;

	public Vector3? Destination;

	public Vector3? MovementVector;

	[Export]
	public float RotationSpeedInterval = 0.1f;

	[Export]
	public float CurrentRotationSpeed = 0.0f;

	[Export]
	public float MaxRotationSpeed = 0.999f;

	[Export]
	public float MaxSpeed = 5f;

	[Export]
	public Vector3 Velocity = new Vector3(0, 0, 0);

	[Export]
	public float Acceleration = 0.2f;

	public Vector3[] Path = null;
	public Transform? TransformPath = null;
	public int CurrentPathIndex = 0;

	private bool haveMoreInPath => Path != null && Path.Length > CurrentPathIndex + 1;

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

		if (Destination == null)
		{
			if (Path == null)
				return;
			else if (Path != null)
			{
				if(!GetNextPathPartIfAvailable())
				{
					return;
				}
			}
		}

		var playerIsFullyRotated = Rotate(delta);

		if(playerIsFullyRotated)
		{
			Move(delta);
		}
	}

	private bool GetNextPathPartIfAvailable()
	{
		CurrentPathIndex++;

		if (CurrentPathIndex >= Path.GetLength(0))
		{
			Path = null;
			CurrentPathIndex = 0;
			GD.Print("Path fin");
			return false;
		}
		else
		{
			GD.Print($"Fetching path #{CurrentPathIndex}");
			InitialiseVariablesForNextTargetDestination(Path[CurrentPathIndex]);
			return true;
		}
	}

	public bool Rotate(float delta)
	{
		if (RotationTarget == null)
			return true;


		if (RotationMatchesTarget())
		{
			ClearRotation();
			return true;
		}
		else
		{
			InterpolateAndRotate(delta);
			return false;
		}
	}

	private void Move(float delta)
	{
		if (Destination == null)
			return;

		var distance = Destination.Value.DistanceTo(Transform.origin);
		var atSamePoint = distance <= 0.5f;
		
		if(atSamePoint)
		{
			if (haveMoreInPath)
				GetNextPathPartIfAvailable();
			else
				ClearDestination();
		}
		else
		{
			//GD.Print(distance);
			InterpolateAndMove(delta);
		}
	}

	private void ClearDestination()
	{
		GD.Print($"Reached destination - we are at {Transform.origin} and destination is {Destination.Value}");
		Transform = new Transform(Transform.basis, Destination.Value);
		Destination = null;

		if (!haveMoreInPath)
		{
			GD.Print("Path over - clearing velocity");
			Velocity = new Vector3(0, 0, 0);
		}
	}

	private void InterpolateAndMove(float delta)
	{
		var newVelocity = Velocity + MovementVector.Value * Acceleration * delta;

		if (newVelocity.Length() < MaxSpeed)
		{
			Velocity = newVelocity;
		}

		var oldOrigin = Transform.origin;

		var collision = MoveAndCollide(Velocity);

		if(collision != null)
		{
			HandleCollision(collision, oldOrigin);
		}

	}

	private void HandleCollision(KinematicCollision collision, Vector3 oldOrigin)
	{
		if (collision.Position.x == 0 && collision.Position.z == 0)
			return;
		Transform = new Transform(Transform.basis, oldOrigin);
		ClearDestination();
	}

	private void InterpolateAndRotate(float delta)
	{
		CurrentRotationSpeed += RotationSpeedInterval * delta;

		if (CurrentRotationSpeed > MaxRotationSpeed)
		{
			CurrentRotationSpeed = MaxRotationSpeed;
		}

		if (RotationTarget.HasValue)
        {
            try
            {
                Transform = Transform.InterpolateWith(RotationTarget.Value, CurrentRotationSpeed);
            }
            catch (Exception ex)
            {
				GD.Print(ex.Message + " - " + CurrentRotationSpeed + " - " + RotationTarget.Value.origin);
            }
        }
    }

	private void ClearRotation()
	{
		GD.Print("Rotated towards target");
		Transform = Transform.LookingAt(Destination.Value, new Vector3(0, 100, 0));
		RotationTarget = null;
		CurrentRotationSpeed = 0.0f;
	}

	private bool RotationMatchesTarget()
	{
		CalculateMovementVector();
		var destinationVector = Transform.basis.z.Normalized();

		var dotProduct = MovementVector.Value.Dot(destinationVector);

		var lookingAtDestination = dotProduct <= -0.99999f;

		//GD.Print(dotProduct);
		return lookingAtDestination;
	}

	private Vector3 CalculateMovementVector()
	{
		MovementVector = (Destination.Value - Transform.origin).Normalized();
		return MovementVector.Value;
	}

	private void _On_MoveTo(Node character, Vector3[] path)
	{
		Path = path;
		CurrentPathIndex = 0;
		GD.Print($"received new path - {string.Join(",", path)}");

		GD.Print(path[0]);
		InitialiseVariablesForNextTargetDestination(path[0]);
	}

	private void InitialiseVariablesForNextTargetDestination(Vector3 target)
	{
		Destination = new Vector3(target.x, Transform.origin.y, target.z);
		GD.Print($"First destination is {target}");
		MovementVector = CalculateMovementVector();
		RotationTarget = Transform.LookingAt(Destination.Value, Vector3.Up);

		Velocity = new Vector3(0, 0, 0);
		GD.Print($"Moving to next part in path -  {Destination.Value}");
	}
}
