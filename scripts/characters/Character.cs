using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.characters
{
	public class Character : Spatial
	{
		[Signal]
		public delegate void _Character_Ready(Node character);

		public Node Body;

		public Character()
		{
		}

		[Export]
		public CharacterStats Stats;

		public bool IsActive = false;

		public bool IsDisposing = false;

		public override void _Ready()
		{
			base._Ready();

			Body = GetNode("KinematicBody");
			AddToGroup("playerCharacters");
			EmitSignal("_Character_Ready", this);

			Body.Connect(SignalNames.Characters.REACHED_PATH_PART, Stats, SignalNames.Characters.REACHED_PATH_PART_METHOD);
			Body.GetNode<ColouredBox>("CSGBox").SetColour(Stats);
		}

        public override void _Process(float delta)
        {
            base._Process(delta);
			if(IsDisposing)
            {
				GetParent().RemoveChild(this);
				Dispose();
			}
        }

        public void _On_Character_ReceiveDamage(int damage, Node origin)
        {
			Stats.AddHealth(-damage);
			GD.Print("Being attacked");
			if(Stats.CurrentHealth <= 0)
            {
				GD.Print($"No health - disposing");
				IsDisposing = true;
				QueueFree();
            }
        }

	}
}

