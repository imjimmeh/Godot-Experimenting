using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.ui
{
	public class WorldCenteredControl : Node
	{
		[Export]
		public float XOffset {get; private set; } = 0.0f;

		[Export]
		public float YOffset { get; private set; } = 0.0f;

		private Control parentControl;
		private Vector3? worldPositionToCenterOn;
		private Camera camera;

		private bool isDisposing = false;


		public void Initialise(Vector3 worldPositionToCenterOn, Camera camera)
		{
			this.worldPositionToCenterOn = worldPositionToCenterOn;
			this.camera = camera;
			this.parentControl = GetParent<Control>();

			this.parentControl.Connect("tree_exiting", this, nameof(DisposeThis));
		}

		public void SetPosition(Vector3 position)
		{
			worldPositionToCenterOn = position;
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			if (isDisposing || worldPositionToCenterOn == null)
				return;

			var newPosition = GetUnprojectedPosition();
			newPosition = new Vector2(newPosition.x + XOffset, newPosition.y + YOffset);

			parentControl.RectPosition = newPosition;
		}

		private Vector2 GetUnprojectedPosition() => camera.UnprojectPosition(worldPositionToCenterOn.Value);

		private void DisposeThis()
		{
			worldPositionToCenterOn = null;
			camera = null;
			isDisposing = true;
			CallDeferred("queue_free");
		}
    }
}
