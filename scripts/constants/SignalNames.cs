using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.constants
{
	public static class SignalNames
	{
		public static class Characters
		{
			private const string prefix = "_Character_";

			public const string CLICKED_ON = prefix + SharedParts.CLICKED_ON;
			public const string MOVE_TO = prefix + SharedParts.MOVE_TO;
			public const string SELECTED = prefix + SharedParts.SELECTED;
			public const string SELECTION_CLEARED = prefix + SharedParts.SELECTION_CLEARED;
		}

		public static class World
		{
			private const string prefix = "_World_";

			public const string CLICKED_ON = prefix + SharedParts.CLICKED_ON;
		}

		public static class SharedParts
		{
			public const string CLICKED_ON = "ClickedOn";
			public const string MOVE_TO = "MoveTo";
			public const string SELECTED = "Selected";
			public const string SELECTION_CLEARED = "SelectionCleared";
		}
	}
}
