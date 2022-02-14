using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.characters
{
	public class Character : Node
	{
		[Signal]
		public delegate void _Character_Disposing(Node character);

		[Signal]
		public delegate void _Character_Ready(Node character);

		[Signal]
		public delegate void _Character_ReceivedDamage(Node character, int damage, Node origin, bool killingBlow);

		public Node Body;
		public MovingKinematicBody ProperBody;

		public Character()
		{
		}

		[Export]
		public CharacterStats Stats;

		public bool IsActive = false;

		public bool IsDisposing = false;

		private bool IsHighlighted = false;

		public override void _Ready()
		{
			base._Ready();

			GetChildNodeFields();

			AddToGroup(GroupNames.CHARACTER);
			ConnectSignals();
			ProperBody.CharacterMesh.SetColour(Stats);

			EmitSignal(SignalNames.Characters.READY, this);
		}

		private void GetChildNodeFields()
		{
			Body = GetNode("KinematicBody");
			ProperBody = Body as MovingKinematicBody;
		}

		public override void _Process(float delta)
		{
			base._Process(delta);

			if (!IsDisposing && IsSetActiveButFinishedTurn())
				IsActive = false;
			if (IsDisposing)
			{
				GetParent().RemoveChild(this);
				IsDisposing = false;
			}
		}

		private bool IsSetActiveButFinishedTurn() => IsActive && ProperBody.MovementStats.AmountLeftToMoveThisTurn == 0;

		public void _On_Character_ReceiveDamage(int damage, Node origin)
		{
			Stats.AddHealth(-damage);

			var characterStillAlive = IsAlive();

			EmitSignal(SignalNames.Characters.RECEIVED_DAMAGE, this, damage, origin, !characterStillAlive);

			if (!characterStillAlive)
			{
				InitialiseDispose();
			}
		}

		public void ConnectSignals()
		{
			Connect(SignalNames.Characters.READY, Body.GetNode("CharacterMovementGuide"), SignalNames.Characters.READY_METHOD);
			ProperBody.GetNode("PathMover").Connect(SignalNames.Characters.REACHED_PATH_PART, ProperBody.MovementStats, SignalNames.Characters.REACHED_PATH_PART_METHOD);
			ProperBody.CharacterMesh.SetParent(this);
		}

		public void ResetTurnStats()
		{
			ProperBody.MovementStats.ResetMovement();
			IsActive = false;
		}

		private void InitialiseDispose()
		{
			EmitSignal("_Character_Disposing", this);
			GD.Print($"No health - disposing");

			IsDisposing = true;
			QueueFree();
		}

		private bool IsAlive() => Stats.CurrentHealth > 0;
	}
}
