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

		private void _On_Character_FaceIconClicked(Node character)
        {
			if(character is Spatial spatial)
            {
				var body = spatial.GetNode<Spatial>("KinematicBody");
				GD.Print($"Received camera move signal");
                MoveToSpatialPostion(body);

            }
        }

        private void MoveToSpatialPostion(Spatial spatial)
        {
            Vector3 target = new Vector3(spatial.GlobalTransform.origin.x, GlobalTransform.origin.y, spatial.GlobalTransform.origin.z);

			//GD.Print($"Moving to {target}");
            GlobalTransform = new Transform(GlobalTransform.basis, target);
        }
    }
}
