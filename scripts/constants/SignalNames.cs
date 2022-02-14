using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.constants
{
	public class SignalNames
	{
		public class Cameras
        {
			private const string prefix = "_Camera_";

			public const string MOVE_TO_POSITION = prefix + "MoveToPosition";
			public const string MOVE_TO_POSITION_METHOD = SharedParts.ON + MOVE_TO_POSITION;
        }
		public class Characters
		{
			private const string prefix = "_Character_";

			public const string CLICKED_ON = prefix + SharedParts.CLICKED_ON;
			public const string CLICKED_ON_METHOD = SharedParts.ON + CLICKED_ON;

			public const string DISPOSING = prefix + SharedParts.DISPOSING;
			public const string DISPOSING_METHOD = SharedParts.ON + DISPOSING;

			public const string MOVE_ORDER = prefix + SharedParts.MOVE_ORDER;
			public const string MOVE_ORDER_METHOD = SharedParts.ON + MOVE_ORDER;

			public const string MOVE_TO = prefix + SharedParts.MOVE_TO;
			public const string MOVE_TO_METHOD = SharedParts.ON + MOVE_TO;

			public const string MOVEMENT_FINISHED = prefix + SharedParts.MOVEMENT_FINISHED;
			public const string MOVEMENT_FINISHED_METHOD = SharedParts.ON + MOVEMENT_FINISHED;

			public const string READY = prefix + SharedParts.READY;
			public const string READY_METHOD = SharedParts.ON + READY;

			public const string RECEIVED_DAMAGE = prefix + SharedParts.RECEIVED_DAMAGE;
			public const string RECEIVED_DAMAGE_METHOD = SharedParts.ON + RECEIVED_DAMAGE;

			public const string REACHED_PATH_PART = prefix + SharedParts.REACHED_PATH_PART;
			public const string REACHED_PATH_PART_METHOD = SharedParts.ON + REACHED_PATH_PART;

			public const string SELECTED = prefix + SharedParts.SELECTED;
			public const string SELECTED_METHOD = SharedParts.ON + SELECTED;

			public const string SELECTION_CLEARED = prefix + SharedParts.SELECTION_CLEARED;
			public const string SELECTION_CLEARED_METHOD = SharedParts.ON + SELECTION_CLEARED;

			public const string TURN_FINISHED = prefix + SharedParts.TURN_FINISHED;
			public const string TURN_FINISHED_METHOD = SharedParts.ON + TURN_FINISHED;
        }

        public class MovementGuide
        {
			private const string CELL_PREFIX = "_Cell";

			public const string CELL_CALCULATE_VISIBLITY = "_Character_MoveGuide_CalculateCellVisiblity";

			public const string CLICKED_ON = "_" + SharedParts.CLICKED_ON;
			public const string CLICKED_ON_METHOD = SharedParts.ON + "_Cell_Clicked";

			public const string CELL_MOUSE_ENTERED = "_Mouse_Entered";
			public const string CELL_MOUSE_ENTERED_METHOD = SharedParts.ON + CELL_PREFIX + CELL_MOUSE_ENTERED;

			public const string CELL_MOUSE_EXITED = "_Mouse_Exited";
			public const string CELL_MOUSE_EXITED_METHOD = SharedParts.ON + CELL_PREFIX + CELL_MOUSE_EXITED;


        }

        public class Loading
        {
			public const string CHARACTERS_SPAWNED = "_Characters_Spawned";
			public const string CHARACTERS_SPAWNED_METHOD = SharedParts.ON + CHARACTERS_SPAWNED;
		}

		public class State
        {
			public const string TURN_CHANGE = "_Turn_Changed";
			public const string TURN_CHANGE_METHOD = "_On_Turn_Change";

		}
        public class World
		{
			private const string prefix = "_World_";

			public const string CLICKED_ON = prefix + SharedParts.CLICKED_ON;
			public const string CLICKED_ON_METHOD = SharedParts.ON + CLICKED_ON;
		}

		public class SharedParts
		{
			public const string CLICKED_ON = "ClickedOn";
			public const string DISPOSING = "Disposing";
			public const string MOVE_ORDER = "MoveOrder";
			public const string MOVE_TO = "MoveTo";
			public const string MOVEMENT_FINISHED = "FinishedMoving";
			public const string REACHED_PATH_PART = "ReachedPathPart";
			public const string READY = "Ready";
			public const string RECEIVED_DAMAGE = "ReceivedDamage";
			public const string SELECTED = "Selected";
			public const string SELECTION_CLEARED = "SelectionCleared";
			public const string TURN_FINISHED = "TurnFinished";
			public const string ON = "_On";
		}
	}
}
