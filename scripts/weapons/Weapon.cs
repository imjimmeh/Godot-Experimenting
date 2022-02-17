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
        public bool IsMelee => Range == 1;
        public bool UsesAmmo => AttacksPerTurn > 0;

        public bool TryAttack(out int damage)
        {
            damage = 0;
             if (!CanAttack)
                return false;

            damage = GetAttackDamage();

            if(!IsMelee)
                DecreaseAmmo();
                
            return true;
        }

        public int GetAttackDamage() => RandomHelper.RNG.RandiRange(MinDamage, MaxDamage);

        public void ResetTurnStats()
        {
            AttacksLeftThisTurn = AttacksPerTurn;
        }

        private void DecreaseAmmo() => AttacksLeftThisTurn--;

        private void SetAmmo(int ammo) => AttacksLeftThisTurn = ammo;
        
    }
}
