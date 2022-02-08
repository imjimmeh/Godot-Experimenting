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
				Hide();
			else
			{
				Show();
			}
		}

		public void CalculateVisiblity(Vector3 parentOrigin)
		{
			var worldPosition = Transform.origin + parentOrigin;
			var shouldHide = worldPosition.x < 1 || worldPosition.x >= 50 || worldPosition.z < 1 || worldPosition.z >= 50;

			if (!shouldHide)
			{
				shouldHide = PositionIsOccupied(worldPosition);

				if(shouldHide)
				{
					GD.Print($"And we are at {Transform.origin}");
				}
			}

			SetVisiblity(!shouldHide);
		}

		private bool PositionIsOccupied(Vector3 worldPosition)
		{
			(int intx, int inty) = ((int)worldPosition.x, (int)worldPosition.z);

			var astarPoint = AStar.GetPointInfoForVector3(worldPosition);
			var occupied = astarPoint.OccupyingNode != null;

			if (occupied)
			{
				GD.Print($"{intx}, {inty} has {astarPoint.OccupyingNode?.Name} in it");
			}

			return occupied;
		}
	}
}

