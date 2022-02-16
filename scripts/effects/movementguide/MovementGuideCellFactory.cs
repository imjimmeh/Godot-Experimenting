using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.effects.movementguide
{
    public static class MovementGuideCellFactory
    {
        public static Quat DefaultRotation = new Quat(0, 100, 0, 0);

		public static CharacterMovementGuideCell CreateMeshInstanceForPosition(this CharacterMovementGuide guide, Vector3 position)
		{
			var meshInstance = guide.MovementGuideCellScene.Instance<CharacterMovementGuideCell>();
			meshInstance.Mesh.ResourceLocalToScene = true;

			meshInstance.Transform = new Transform(DefaultRotation, position);
			meshInstance.AStar = guide.AStar;

			meshInstance.Name = position.ToString();
			return meshInstance;
		}

		public static CharacterMovementGuide ConnectCellSignals(this CharacterMovementGuide guide, CharacterMovementGuideCell cell)
        {
			cell.Connect(SignalNames.MovementGuide.CELL_MOUSE_ENTERED, guide, SignalNames.MovementGuide.CELL_MOUSE_ENTERED_METHOD);
			cell.Connect(SignalNames.MovementGuide.CELL_MOUSE_EXITED, guide, SignalNames.MovementGuide.CELL_MOUSE_EXITED_METHOD);

			return guide;
		}
	}
}
