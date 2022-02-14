using FaffLatest.scripts.shared;
using Godot;
using static FaffLatest.scripts.constants.SignalNames;

namespace FaffLatest.scripts.movement
{
	public class PathMover : Node
	{
		[Signal]
		public delegate void _Character_ReachedPathPart(Node character, Vector3 part);

		private Node character;

		public bool HaveMoreInPath => Path != null && CurrentPathIndex < Path.Length;


		public Spatial KinematicBody;
		public Vector3[] Path = null;
		public Vector3 CurrentTarget => Path[CurrentPathIndex];


		public int CurrentPathIndex = 0;


        public override void _Ready()
		{
			base._Ready();

			character = GetNode("../../");
			KinematicBody = GetNode<Spatial>("../");
		}

		public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
		}

        public bool TryGetNextPathPart(out Vector3 nextPath)
		{
			var notFirstPart = CurrentPathIndex > -1;

			if (notFirstPart)
			{
				EmitSignal(Characters.REACHED_PATH_PART, this, KinematicBody.Transform.origin.Round());
			}

			CurrentPathIndex++;
			nextPath = Vector3.Zero;

			var pathFinished = CurrentPathIndex >= Path.Length;

			if (pathFinished)
			{
				ClearPath();
			}
			else
			{
				nextPath = CurrentTarget.CopyYValue(KinematicBody.Transform.origin);
			}

			return !pathFinished;
		}

		public void ClearPath()
		{
			Path = null;
			CurrentPathIndex = -1;
		}

		private void SetInitialMovementVariables()
		{
			CurrentPathIndex = -1;
		}

		public void MoveWithPath(Vector3[] path)
		{
			Path = path;
			SetInitialMovementVariables();
		}

        private void _On_Character_MoveTo(Node character, Vector3[] path)
        {
            if (this.character != character)
                return;

            MoveWithPath(path);
        }
    }
}
