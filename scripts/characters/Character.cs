using Godot;

namespace FaffLatest.scripts.characters
{
	public class Character : Spatial
	{
		[Signal]
		public delegate void _Character_Ready(Node character);

		public Node CharacterKinematicBody;

		public Character()
		{
		}

		[Export]
		public CharacterStats Stats;

		public override void _Ready()
		{
			base._Ready();

			CharacterKinematicBody = GetNode("KinematicBody");
			AddToGroup("playerCharacters");
			EmitSignal("_Character_Ready", this);
		}
	}
}

