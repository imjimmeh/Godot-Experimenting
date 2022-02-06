using FaffLatest.scripts.characters;
using Godot;
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
	public float MaxRotationSpeed = 1f;

	[Export]
	public float MaxSpeed = 5f;

	[Export]
	public Vector3 Velocity = new Vector3(0, 0, 0);

	[Export]
	public float Acceleration = 0.2f;

	public override void _Ready()
	{
		base._Ready();

		if(Parent == null)
		{
			Parent = GetNode<Node>("../");
		}    
	}

	public override void _InputEvent(Object camera, InputEvent inputEvent, Vector3 position, Vector3 normal, int shapeIdx)
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
			return;

		var playerIsFullyRotated = Rotate(delta);

		if(playerIsFullyRotated)
		{
			Move(delta);
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
			ClearDestination();
		}
		else
		{
			GD.Print(distance);
			InterpolateAndMove(delta);
		}
	}

	private void ClearDestination()
	{
		GD.Print($"Reached destination - we are at {Transform.origin} and destination is {Destination.Value}");
		Transform = new Transform(Transform.basis, Destination.Value);
		Destination = null;
		Velocity = new Vector3(0,0,0);
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

		Transform = Transform.InterpolateWith(RotationTarget.Value, CurrentRotationSpeed);
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

		GD.Print(dotProduct);
		return lookingAtDestination;
	}

	private Vector3 CalculateMovementVector()
	{
		MovementVector = (Destination.Value - Transform.origin).Normalized();
		return MovementVector.Value;
	}

	private void _On_MoveTo(Node character, Vector3 target)
	{
		Destination = new Vector3(target.x, Transform.origin.y, target.z).Round();
		MovementVector = CalculateMovementVector();
		RotationTarget = Transform.LookingAt(Destination.Value, Vector3.Up);
		Velocity = new Vector3(0, 0, 0);
		GD.Print($"Received move command towards {Destination}");
	}
}
