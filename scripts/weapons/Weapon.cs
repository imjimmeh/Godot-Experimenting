using FaffLatest.scripts.shared;
using Godot;

namespace FaffLatest.scripts.weapons
{
    public class Weapon : Reference
    {
        public Weapon()
        {
        }

        public Weapon(string name = null, int minDamage = 0, int maxDamage = 0, int attacksPerTurn = 0, int range = 0)
        {
            Name = name;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            AttacksPerTurn = attacksPerTurn;
            AttacksLeftThisTurn = AttacksPerTurn;
            Range = range;
        }

        public Weapon(Weapon weapon)
        {
            Name = weapon.Name;
            MinDamage = weapon.MinDamage;
            MaxDamage = weapon.MaxDamage;
            AttacksPerTurn = weapon.AttacksPerTurn;
            AttacksLeftThisTurn = weapon.AttacksPerTurn;
            Range = weapon.Range;
        }

        [Export]
        public string Name { get; private set; }

        [Export]
        public int MinDamage { get; private set; }

        [Export]
        public int MaxDamage { get; private set; }

        [Export]
        public int AttacksPerTurn { get; private set; }

        [Export]
        public int Range { get; private set; }

        public int AttacksLeftThisTurn { get; private set; }

        public bool CanAttack => HaveAttacksLeft;
        public bool HaveAttacksLeft => AttacksLeftThisTurn > 0;
        public bool UsesAmmo => AttacksPerTurn > 0;

        public AttackResult TryAttack(out int damage)
        {
            damage = 0;

            if (!HaveAttacksLeft || !CanAttack)
                return AttackResult.OutOfAttacksForTurn;

            damage = GetAttackDamage();

            DecreaseAttacksLeft();

            return AttackResult.Success;
        }

        public bool WithinAttackRange(float enemyDistance) => Range + 0.5 >= enemyDistance;

        public int GetAttackDamage() => RandomHelper.RNG.RandiRange(MinDamage, MaxDamage);

        public void ResetTurnStats() => AttacksLeftThisTurn = AttacksPerTurn;

        private void DecreaseAttacksLeft() => AttacksLeftThisTurn--;

        private void SetAttacksPerLeft(int attacksLeft) => AttacksLeftThisTurn = attacksLeft;

        public void EndTurn() => SetAttacksPerLeft(0);

    }
}
