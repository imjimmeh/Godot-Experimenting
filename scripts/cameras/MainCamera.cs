using FaffLatest.scripts.constants;
using Godot;
using System;

namespace FaffLatest.scripts.cameras
{
	public class MainCamera : Camera
	{
		Vector3 velocity;

		[Export]
		public float Speed { get; set; } = 4.0f;

		private float NegativeSpeed;

		public Vector3 Direction { get; set; }

		public override void _Ready()
		{
			NegativeSpeed = Speed;

			GetNode(NodeReferences.Systems.INPUT_MANAGER).Connect(SignalNames.Cameras.MOVE_TO_POSITION, this, SignalNames.Cameras.MOVE_TO_POSITION_METHOD);
			base._Ready();
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);


			if (Direction.Length() == 0)
			{
				velocity = Vector3.Zero;
				return;
			}

			CalculateVelocity(delta);
			Move();
		}

		private void CalculateVelocity(float delta)
		{
			velocity = Direction * delta * Speed;
		}

		public override void _Input(InputEvent inputEvent)
		{
			(float x, float y, float z) = (Direction.x, Direction.y, Direction.z);

			if (inputEvent.IsActionPressed("camera_left"))
				x = -1;
			else if (inputEvent.IsActionPressed("camera_right"))
				x = 1;
			else if (inputEvent.IsActionReleased("camera_left") || inputEvent.IsActionReleased("camera_right"))
				x = 0;


			if (inputEvent.IsActionPressed("camera_up"))
				z = -1;
			else if (inputEvent.IsActionPressed("camera_down"))
				z = 1;
			else if (inputEvent.IsActionReleased("camera_up") || inputEvent.IsActionReleased("camera_down"))
				z = 0;

			Direction = new Vector3(x, y, z);

		}

		private void Move()
		{
			var targetPos = Transform.origin + velocity;
			var targetTransform = new Transform(Transform.basis, targetPos);
			Transform = Transform.InterpolateWith(targetTransform, 1.0f);
		}

		private void _On_Camera_MoveToPosition(Vector3 position)
        {
			MoveToSpatialPostion(position);
		}

        private void MoveToSpatialPostion(Vector3 position)
        {
            Vector3 target = new Vector3(position.x, GlobalTransform.origin.y, position.z);

			//GD.Print($"Moving to {target}");
            GlobalTransform = new Transform(GlobalTransform.basis, target);
        }
    }
}
