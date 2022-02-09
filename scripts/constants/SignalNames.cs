using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.constants
{
	public class SignalNames
	{
		public class Characters
		{
			private const string prefix = "_Character_";

			public const string CLICKED_ON = prefix + SharedParts.CLICKED_ON;
			public const string CLICKED_ON_METHOD = SharedParts.ON + CLICKED_ON;

			public const string MOVE_TO = prefix + SharedParts.MOVE_TO;
			public const string MOVE_TO_METHOD = SharedParts.ON + MOVE_TO;

			public const string MOVEMENT_FINISHED = prefix + SharedParts.MOVEMENT_FINISHED;
			public const string MOVEMENT_FINISHED_METHOD = SharedParts.ON + MOVEMENT_FINISHED;

			public const string REACHED_PATH_PART = prefix + SharedParts.REACHED_PATH_PART;
			public const string REACHED_PATH_PART_METHOD = SharedParts.ON + REACHED_PATH_PART;

			public const string SELECTED = prefix + SharedParts.SELECTED;
			public const string SELECTED_METHOD = SharedParts.ON + SELECTED;

			public const string SELECTION_CLEARED = prefix + SharedParts.SELECTION_CLEARED;
			public const string SELECTION_CLEARED_METHOD = SharedParts.ON + SELECTION_CLEARED;

			public const string TURN_FINISHED = prefix + SharedParts.TURN_FINISHED;
			public const string TURN_FINISHED_METHOD = SharedParts.ON + TURN_FINISHED;
		}

		public class World
		{
			private const string prefix = "_World_";

			public const string CLICKED_ON = prefix + SharedParts.CLICKED_ON;
		}

		public class SharedParts
		{
			public const string CLICKED_ON = "ClickedOn";
			public const string MOVE_TO = "MoveTo";
			public const string MOVEMENT_FINISHED = "FinishedMoving";
			public const string REACHED_PATH_PART = "ReachedPathPart";
			public const string SELECTED = "Selected";
			public const string SELECTION_CLEARED = "SelectionCleared";
			public const string TURN_FINISHED = "TurnFinished";
			public const string ON = "_On";
		}
	}
}
