using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.shared;
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
		public float HeightToDisplayAt = 0.5f;

		public override void _Ready()
		{
			base._Ready();
			GetNode(NodeReferences.Systems.INPUT_MANAGER).Connect(SignalNames.Characters.MOVE_TO, this, SignalNames.Characters.MOVE_TO_METHOD);
		}

		private void _On_Character_MoveTo(Node character, Vector3[] path)
		{
			var body = character.GetNode("KinematicBody");
			var pathMover = body.GetNode("PathMover");

			InitialisePathArray(path);

			for (var x = 0; x < path.Length; x++)
			{
				var targetPosition = path[x].WithValues(y: HeightToDisplayAt);
				var newMesh = MovementPathFactory.CreateMeshInstanceForPoint(targetPosition, SetPlaneVariables());

				SetMeshVariables(body, newMesh);
				ConnectSignals(pathMover, newMesh);
				AddChild(newMesh);

				existingPath[x] = newMesh;
			}
		}

		private Action<PlaneMesh> SetPlaneVariables() => plane =>{
			plane.Material = MeshMaterial;
		};


		private static void ConnectSignals(Node pathMover, CharacterMovementPath newMesh)
		{
			//GD.Print($"Connected mesh");
			pathMover.Connect(SignalNames.Characters.REACHED_PATH_PART, newMesh, SignalNames.Characters.REACHED_PATH_PART_METHOD);
		}

		private void SetMeshVariables(Node body, CharacterMovementPath newMesh)
		{
			newMesh.SetParentNode(this);
			newMesh.SetCharacterBody(body);
		}

		private void InitialisePathArray(Vector3[] path)
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
