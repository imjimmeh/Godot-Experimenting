using FaffLatest.scripts.constants;
using FaffLatest.scripts.input;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.shared;
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
			CallDeferred("hide");
		}

        public override void _Process(float delta)
		{
			base._Process(delta);
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
				EmitSignal(SignalNames.MovementGuide.CLICKED_ON, this, mouseButtonEvent);
			}
		}

		private void ConnectSignals()
		{
			Connect(SignalNames.MovementGuide.CLICKED_ON, GetParent(), SignalNames.MovementGuide.CLICKED_ON_METHOD);
		}

		public void CalculateVisiblity(int movementDistanceLeft, int weaponRange)
        {
            if (AStar.OutsideWorldBounds(this.GlobalTransform.origin, this.GlobalTransform.origin))
                return;

            if (IsInAttackRange(weaponRange) && !AStar.IsPointDisabled(this.GlobalTransform.origin))
            {
				Visible = true;
				return;
            }

            GetPathAndSetVisiblity(movementDistanceLeft);
        }

        private bool IsInAttackRange(int weaponRange)
        {
			var distanceToParent = parent.GlobalTransform.origin.DistanceToIgnoringHeight(this.GlobalTransform.origin);
            var isInAttackRange = distanceToParent <= weaponRange + 0.5f;

			material.SetShaderParam("isInAttackRange", isInAttackRange);
			return isInAttackRange;
        }

        private bool GetPathAndSetVisiblity(int movementDistanceLeft)
		{
            var result = AStar.TryGetMovementPath(parent.GlobalTransform.origin, GlobalTransform.origin, movementDistanceLeft);

            var isVisible = result != null && result.CanFindPath && !result.NotEnoughMovementDistanceToFullyReach;

            SetVisiblity(isVisible);
			return isVisible;
		}

        private void SetVisiblity(bool isVisible)
        {
			if(isVisible)
			{
				Show();
			}
			else
			{
				Hide();
			}
        }

        private bool IsOutsideWorldBounds()
			=> GlobalTransform.origin.x < 1 || GlobalTransform.origin.x >= AStar.Width || GlobalTransform.origin.z < 1 || GlobalTransform.origin.z >= AStar.Length;
	}
}

