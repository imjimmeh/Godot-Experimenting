using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;
using System;

namespace FaffLatest.scripts.effects
{
	public class CharacterMovementPathManager : Spatial
	{
		private CharacterMovementPath[] existingPath;

		[Export]
		public Material MeshMaterial;

		[Export(PropertyHint.Length, "How high the path should be displayed")]
		public float HeightToDisplayAt = 1f;

		public override void _Ready()
		{
			base._Ready();
			GetNode("/root/Root/Systems/InputManager").Connect(SignalNames.Characters.MOVE_TO, this, "_On_Character_MoveTo");
		}

		private void _On_Character_MoveTo(Node character, MovementPathNode[] path)
		{
			var body = character.GetNode("KinematicBody");

			InitialisePathArray(path);

			for (var x = 0; x < path.Length; x++)
			{
				var newMesh = MovementPathFactory.CreateMeshInstanceForPoint(path[x].Destination, SetPlaneVariables());

				SetMeshVariables(body, newMesh);
				ConnectSignals(body, newMesh);
				AddChild(newMesh);

				existingPath[x] = newMesh;
			}
		}

		private Action<PlaneMesh> SetPlaneVariables() => plane =>{
			plane.Material = MeshMaterial;
		};


		private static void ConnectSignals(Node body, CharacterMovementPath newMesh)
		{
			//GD.Print($"Connected mesh");
			body.Connect(SignalNames.Characters.REACHED_PATH_PART, newMesh, SignalNames.Characters.REACHED_PATH_PART_METHOD);
		}

		private void SetMeshVariables(Node body, CharacterMovementPath newMesh)
		{
			newMesh.SetParentNode(this);
			newMesh.SetCharacterBody(body);
		}

		private void InitialisePathArray(MovementPathNode[] path)
		{
			DisposeMesh();
			existingPath = new CharacterMovementPath[path.Length];
		}

		private void _On_Character_FinishedMoving(Node character, Vector3 newPosition)
		{
			//GD.Print("Disposing");
			DisposeMesh();
		}

		private void DisposeMesh()
		{
			if (existingPath == null)
				return;

			foreach(var part in existingPath)
			{
				if(part != null)
				{
					if(!part.isHidden)
						part.DisconnectAndRemove();

					part.Dispose();
				}
			}

			existingPath = null;
		}
	}
}
