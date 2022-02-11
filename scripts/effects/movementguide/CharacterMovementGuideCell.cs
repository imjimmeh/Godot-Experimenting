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
		public delegate void _Clicked_On(Node cell);

		[Signal]
		public delegate void _Mouse_Entered(Node node);

		[Signal]
		public delegate void _Mouse_Exited(Node node);

		public bool MouseIsOver = false;
		public bool IsPartOfPath = false;

		public AStarNavigator AStar;

		public override void _Ready()
		{
			base._Ready();
			ConnectSignals();

			material = Mesh.SurfaceGetMaterial(0) as ShaderMaterial;
			Connect("_Clicked_On", GetNode("../"), "_On_Cell_Clicked");
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
			EmitSignal("_Mouse_Entered", this);

			material.SetShaderParam(SHADER_TARGETCELL_PARAM, true);
		}
		
		private void _On_Mouse_Exited()
		{
			MouseIsOver = false;
			EmitSignal("_Mouse_Exited", this);

			material.SetShaderParam("isTargetCell", false);
		}

		private void _on_StaticBody_input_event(Godot.Object camera, InputEvent inputEvent, Vector3 position, Vector3 normal, int shapeIdx)
		{
			if (inputEvent is InputEventMouseButton mouseButtonEvent)
			{
				//GD.Print("World clicked on - emitting signal");
				EmitSignal("_Clicked_On", this, mouseButtonEvent);
			}
		}

		private void ConnectSignals()
		{
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

		public void CalculateVisiblity(float movementDistanceLeft)
		{
			bool shouldHide = IsOutsideWorldBounds() || PositionIsOccupied() || IsOutsideMovementDistance(movementDistanceLeft);
			SetVisiblity(!shouldHide);
		}

		private bool IsOutsideMovementDistance(float movementDistanceLeft)
			=> (Transform.origin).Length() > movementDistanceLeft;


		//TODO: Make static - move to Map/World manager
		private bool IsOutsideWorldBounds()
			=> GlobalTransform.origin.x < 1 || GlobalTransform.origin.x >= 50 || GlobalTransform.origin.z < 1 || GlobalTransform.origin.z >= 50;

		private bool PositionIsOccupied()
		{
			var astarPoint = AStar.GetPointInfoForVector3(GlobalTransform.origin);

			return astarPoint.OccupyingNode != null;
		}
	}
}

