using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.characters
{
	public class Character : Spatial
	{
		[Signal]
		public delegate void _Character_Ready(Node character);

		[Signal]
		public delegate void _Character_ReceivedDamage(Node character, int damage, Node origin);

		public Node Body;
		public MovingKinematicBody ProperBody;

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
			ProperBody = Body as MovingKinematicBody;

			AddToGroup(GroupNames.CHARACTER);
			EmitSignal(SignalNames.Characters.READY, this);

			ProperBody.GetNode("PathMover").Connect(SignalNames.Characters.REACHED_PATH_PART, Stats, SignalNames.Characters.REACHED_PATH_PART_METHOD);
			Body.GetNode<ColouredBox>("CSGBox").SetColour(Stats);
		}

		public override void _Process(float delta)
		{
			base._Process(delta);
			if(IsDisposing)
			{
				GetParent().RemoveChild(this);
			}
		}

		public void _On_Character_ReceiveDamage(int damage, Node origin)
		{
			Stats.AddHealth(-damage);

			EmitSignal(SignalNames.Characters.RECEIVED_DAMAGE, this, damage, origin);
			GD.Print("Being attacked");
			if (NoHealthLeft())
			{
				InitialiseDispose();
			}
		}

		public void ConnectSignals(Node ui)
		{
			Connect(SignalNames.Characters.RECEIVED_DAMAGE, ui, SignalNames.Characters.RECEIVED_DAMAGE_METHOD);
		}

		private void InitialiseDispose()
		{
			GD.Print($"No health - disposing");
			IsDisposing = true;
			QueueFree();
		}

		private bool NoHealthLeft()
		{
			return Stats.CurrentHealth <= 0;
		}
	}
}

