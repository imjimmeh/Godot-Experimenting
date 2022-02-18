using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace FaffLatest.scripts.movement
{
	public class MovingUiControl : Node
	{
		[Export]
		public float MovementSpeed { get; private set; } = 5;

		[Export]
		public int PixelsUpToMove { get; private set; } = 100;

		[Export]
		public float InterpWeight { get; private set; } = 0.5f;

		private Control parent;

		private Vector2 movementVector;

		private Vector2 velocity;

		public MovingUiControl()
		{
		}

		public override void _Ready()
		{
			base._Ready();
			GetAndSetParent();
		}
		
		public void GetAndSetParent(Control parent = null)
		{
			if (this.parent == null)
			{
				this.parent = parent ?? GetParent<Control>();
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			MoveTowardsTarget(delta);
			base._Process(delta);
		}

		public void SetMovementVector(Vector2 movementVector)
		{
			this.movementVector = movementVector;
		}

		private void MoveTowardsTarget(float delta)
		{
			velocity += (movementVector * delta * MovementSpeed);
			var target = parent.RectPosition + velocity;

			parent.RectPosition = parent.RectPosition.LinearInterpolate(target, InterpWeight); 
		}
	}
}
