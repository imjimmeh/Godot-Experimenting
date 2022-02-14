using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace FaffLatest.scripts.movement
{
	public class MoveUpwards : Node
	{
		[Export]
		public float MovementSpeed { get; private set; } = 5;

		[Export]
		public int PixelsUpToMove { get; private set; } = 100;

		[Export]
		public float InterpWeight { get; private set; } = 0.5f;

		private Control parent;

		private Vector2 targetDestination;

		private Vector2 velocity;

		public MoveUpwards()
		{
		}

		public void SetParentField()
		{
			parent = GetParent<Control>();
			if (parent == null)
				return;

			SetTargetDestination();
		}

		public override void _PhysicsProcess(float delta)
		{
			if (parent == null)
			{
				SetParentField();
				return;
			}

			MoveTowardsTarget(delta);
			base._Process(delta);
		}

		public void SetTargetDestination()
		{
			targetDestination = new Vector2(parent.RectPosition.x, -2000);
		}

		private void MoveTowardsTarget(float delta)
		{
			var direction = (targetDestination - parent.RectPosition).Normalized();
			velocity += (direction * delta * MovementSpeed);

			var target = parent.RectPosition + velocity;

			parent.RectPosition = parent.RectPosition.LinearInterpolate(target, InterpWeight); 
		}
	}
}
