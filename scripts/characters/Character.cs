using FaffLatest.scripts.constants;
using FaffLatest.scripts.weapons;
using Godot;

namespace FaffLatest.scripts.characters
{
	public class Character : Node
	{
		[Signal]
		public delegate void _Character_Disposing(Character character);

		[Signal]
		public delegate void _Character_Ready(Character character);

		[Signal]
		public delegate void _Character_ReceivedDamage(Character character, int damage, Vector3 origin, bool killingBlow);

		[Signal]
		public delegate void _Character_Attacking(Character attacker, Character receiver);
		
		public MovingKinematicBody Body;

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
			Body.CharacterMesh.SetColour(Stats);

			EmitSignal(SignalNames.Characters.READY, this);
		}

		private void GetChildNodeFields()
		{
			Body = GetNode<MovingKinematicBody>("KinematicBody");
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

		private bool IsSetActiveButFinishedTurn() => IsActive && Body.MovementStats.AmountLeftToMoveThisTurn == 0;

		public void _On_Character_ReceiveDamage(int damage, Vector3 origin)
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
			Connect(SignalNames.Characters.READY, Body.GetNode("CharacterMovementGuide"), SignalNames.Characters.READY_METHOD);
			Body.GetNode("PathMover").Connect(SignalNames.Characters.REACHED_PATH_PART, Body.MovementStats, SignalNames.Characters.REACHED_PATH_PART_METHOD);
			Body.CharacterMesh.SetParent(this);
		}

		public AttackResult TryAttackTarget(Character target)
		{
			var distanceToTarget = this.DistanceToIgnoringHeight(target);
            
            if(!Stats.EquippedWeapon.WithinAttackRange(distanceToTarget))
				return AttackResult.OutOfRange;

			var attackResult = Stats.EquippedWeapon.TryAttack(out int damage);

			if(attackResult != AttackResult.Success)
				return attackResult;

			target._On_Character_ReceiveDamage(damage, Body.GlobalTransform.origin);
			EmitSignal(nameof(_Character_Attacking), this, target);
			return AttackResult.Success;
		}

		public AttackResult CanAttackTarget(Character target)
		{
			var distanceToTarget = this.DistanceToIgnoringHeight(target);
            
            if(!Stats.EquippedWeapon.WithinAttackRange(distanceToTarget))
				return AttackResult.OutOfRange;

			if (!Stats.EquippedWeapon.HaveAttacksLeft)
                return AttackResult.OutOfAttacksForTurn;

			return AttackResult.CanAttack;
		}
		public void ResetTurnStats()
		{
			Body.MovementStats.ResetMovementForTurn();
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
