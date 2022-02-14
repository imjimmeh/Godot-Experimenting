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
		private Spatial worldObject;
		private Camera camera;

		private bool isDisposing = false;
		public void Initialise(Spatial objectToCenterOn, Camera camera)
		{
			worldObject = objectToCenterOn;
			this.camera = camera;

			objectToCenterOn.GetParent().Connect("_Character_Disposing", this, "_On_WorldObject_Disposing");
		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);

			if (isDisposing)
				return;

			var newPosition = GetUnprojectedPosition();
			newPosition = new Vector2(newPosition.x, newPosition.y - 20);

			RectPosition = newPosition;
		}

		private Vector2 GetUnprojectedPosition() => camera.UnprojectPosition(worldObject.GlobalTransform.origin);

		private void DisposeThis()
		{
			worldObject = null;
			camera = null;
			isDisposing = true;
			QueueFree();
		}

        private void _On_WorldObject_Disposing(Spatial worldObject)
        {
            DisposeThis();
        }
    }
}
