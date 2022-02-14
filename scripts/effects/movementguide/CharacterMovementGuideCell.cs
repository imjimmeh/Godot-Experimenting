using FaffLatest.scripts.constants;
using FaffLatest.scripts.input;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.effects.movementguide
{
	public class CharacterMovementGuideCell : MeshInstance
	{
		private const string SHADER_PATHPART_PARAM = "isPartOfPath";
		private const string SHADER_TARGETCELL_PARAM = "isTargetCell";

		private ShaderMaterial material;

		[Signal]
		public delegate void _ClickedOn(Node cell);

		[Signal]
		public delegate void _Mouse_Entered(Node node);

		[Signal]
		public delegate void _Mouse_Exited(Node node);

		public bool MouseIsOver = false;
		public bool IsPartOfPath = false;

		public AStarNavigator AStar;

		private Spatial parent;

		public override void _Ready()
		{
			base._Ready();
			ConnectSignals();

			material = Mesh.SurfaceGetMaterial(0) as ShaderMaterial;
		}

		public void SetParentCharacterTransform(Spatial parent)
        {
			this.parent = parent;
        }

		public void SetPartOfPath(bool isPathPart)
		{
			IsPartOfPath = isPathPart;
			MouseIsOver = false;

			material.SetShaderParam(SHADER_PATHPART_PARAM, isPathPart);
		}

		private void _On_Mouse_Entered()
		{
			MouseIsOver = true;
			EmitSignal(SignalNames.MovementGuide.CELL_MOUSE_ENTERED, this);

			material.SetShaderParam(SHADER_TARGETCELL_PARAM, true);
		}
		
		private void _On_Mouse_Exited()
		{
			MouseIsOver = false;
			EmitSignal(SignalNames.MovementGuide.CELL_MOUSE_EXITED, this);

			material.SetShaderParam("isTargetCell", false);
		}

		private void _on_StaticBody_input_event(Godot.Object camera, InputEvent inputEvent, Vector3 position, Vector3 normal, int shapeIdx)
		{
			if (inputEvent is InputEventMouseButton mouseButtonEvent)
			{
				//GD.Print("World clicked on - emitting signal");
				EmitSignal(SignalNames.MovementGuide.CLICKED_ON, this, mouseButtonEvent);
			}
		}

		private void ConnectSignals()
		{
			Connect(SignalNames.MovementGuide.CLICKED_ON, GetNode("../"), SignalNames.MovementGuide.CLICKED_ON_METHOD);
		}

		public void SetVisiblity(bool visible)
		{
			if (!visible)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}

		private void CalculateVisiblity(int movementDistanceLeft)
		{
			bool isVisible = false;

			if (IsOutsideWorldBounds())
			{
				SetVisiblity(isVisible);
				return;
			}

			var start = parent.Transform.origin;
            var path = AStar.GetMovementPath(start, GlobalTransform.origin, movementDistanceLeft);
            isVisible = path == null || path.Length > movementDistanceLeft;

            SetVisiblity(!isVisible);
        }

		private bool IsOutsideWorldBounds()
			=> GlobalTransform.origin.x < 1 || GlobalTransform.origin.x >= AStar.Width || GlobalTransform.origin.z < 1 || GlobalTransform.origin.z >= AStar.Length;
	}
}

