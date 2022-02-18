using FaffLatest.scripts.constants;
using FaffLatest.scripts.weapons;
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
			ProperBody = GetNode<MovingKinematicBody>("KinematicBody");
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
			EmitSignal(SignalNames.Characters.RECEIVED_DAMAGE, this, damage, origin, !IsAlive);

			if (!IsAlive)
			{
				InitialiseDispose();
			}
		}

		public void ConnectSignals()
		{
			Connect(SignalNames.Characters.READY, ProperBody.GetNode("CharacterMovementGuide"), SignalNames.Characters.READY_METHOD);
			ProperBody.GetNode("PathMover").Connect(SignalNames.Characters.REACHED_PATH_PART, ProperBody.MovementStats, SignalNames.Characters.REACHED_PATH_PART_METHOD);
			ProperBody.CharacterMesh.SetParent(this);
		}

		public AttackResult TryAttackTarget(Character target)
		{
			var distanceToTarget = this.DistanceToIgnoringHeight(target);
            
            if(!Stats.EquippedWeapon.WithinAttackRange(distanceToTarget))
				return AttackResult.OutOfRange;

			var attackResult = Stats.EquippedWeapon.TryAttack(out int damage);

			if(attackResult != AttackResult.Success)
				return attackResult;

			target._On_Character_ReceiveDamage(damage, this);
			
			return AttackResult.Success;
		}

		public void ResetTurnStats()
		{
			ProperBody.MovementStats.ResetMovementForTurn();
			Stats.EquippedWeapon.ResetTurnStats();
			IsActive = false;
		}

		private void InitialiseDispose()
		{
			EmitSignal("_Character_Disposing", this);
			IsDisposing = true;
			CallDeferred("queue_free");
		}

		private bool IsAlive => Stats.CurrentHealth > 0;
	}
}
