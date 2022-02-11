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
			GD.Print($"Getting next path part");
			CurrentPathIndex++;
			nextPath = Vector3.Zero;

			var pathFinished = CurrentPathIndex >= Path.Length;

			if (pathFinished)
				ClearPath();
			else
			{
				nextPath = new Vector3(CurrentTarget.x, KinematicBody.Transform.origin.y, CurrentTarget.z);
			}

			var notFirstPart = CurrentPathIndex > 0;

			if (notFirstPart)
			{
				EmitSignal(Characters.REACHED_PATH_PART, this, KinematicBody.Transform.origin.Round());

				if (nextPath == null)
					EmitSignal("_Character_MovementFinished");

			}

			GD.Print($"Path is finished is {pathFinished} - not firste part is {notFirstPart} - current index is {CurrentPathIndex} current path is{nextPath} ");

			return !pathFinished;
		}

		public void ClearPath()
		{
			//GD.Print($"Reached destination - we are at {Transform.origin}");
			Path = null;
			CurrentPathIndex = 0;
			//GD.Print($"Snapepd to {snappedVector}");
		}

		private void SetInitialMovementVariables()
		{
			CurrentPathIndex = 0;
			//TryGetNextPathPart(out Vector3 target);
		}

		public void MoveWithPath(Vector3[] path)
		{
			Path = path;
			GD.Print($"Setting path as {string.Join(",", path)}");
			//GD.Print($"Path length is {path.Length}");
			SetInitialMovementVariables();

			//GD.Print($"Received movement command - moving to first part in path -  {Path[0].Destination}");
		}

        private void _On_Character_MoveTo(Node character, Vector3[] path)
        {
            if (this.character != character)
                return;

            MoveWithPath(path);
        }
    }
}
