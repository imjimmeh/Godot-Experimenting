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
			CharacterKinematicBody = GetNode("KinematicBody");
			AddToGroup("characters");

			base._Ready();
		}
    }
}

