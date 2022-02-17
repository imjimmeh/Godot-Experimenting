using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.ui
{
	public class WorldCenteredControl : Control
	{
		private Vector3? worldPositionToCenterOn;
		private Camera camera;

		private bool isDisposing = false;
		public void Initialise(Vector3 worldPositionToCenterOn, Camera camera)
		{
			this.worldPositionToCenterOn = worldPositionToCenterOn;
			this.camera = camera;
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			if (isDisposing || worldPositionToCenterOn == null)
				return;

			var newPosition = GetUnprojectedPosition();
			newPosition = new Vector2(newPosition.x, newPosition.y - 20);

			RectPosition = newPosition;
		}

		private Vector2 GetUnprojectedPosition() => camera.UnprojectPosition(worldPositionToCenterOn.Value);

		private void DisposeThis()
		{
			worldPositionToCenterOn = null;
			camera = null;
			isDisposing = true;
			CallDeferred("queue_free");
		}

        private void _On_WorldObject_Disposing(Spatial worldObject)
        {
            DisposeThis();
        }
    }
}
