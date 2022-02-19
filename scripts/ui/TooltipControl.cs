using Godot;

namespace FaffLatest.scripts.ui{
    public class TooltipControl : Label
    {
        private Control triggeringControl;
        private bool mouseIsOver = false;

        public override void _Ready()
        {
            base._Ready();

            triggeringControl = GetParent<Control>();
            triggeringControl.Connect("mouse_entered", this, "_MouseEntered");
			triggeringControl.Connect("mouse_exited", this, "_MouseExited");
        }

        public override void _Process(float delta)
		{
			base._Process(delta);

			if(mouseIsOver)
			{
				var mousePos = GetGlobalMousePosition();
				RectGlobalPosition = new Vector2(mousePos.x + (Text.Length), mousePos.y + 10);
			}
		}
        
		private void _MouseEntered()
		{
			mouseIsOver = true;
			CallDeferred("show");
		}

		private void _MouseExited()
		{
			mouseIsOver = false;
			CallDeferred("hide");
		}

    }
}