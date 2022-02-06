using Godot;

namespace FaffLatest.scripts.characters
{
	public class Character : Spatial
	{
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
			AddToGroup("characters");
		}
	}
}

