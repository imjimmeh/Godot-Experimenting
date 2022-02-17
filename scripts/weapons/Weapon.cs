using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaffLatest.scripts.shared;
using Godot;
namespace FaffLatest.scripts.weapons
{
    public class Weapon : Reference
    {
        public Weapon()
        {
        }

        public Weapon(string name = null, int minDamage = 0, int maxDamage = 0, int ammoPerClip = 0, int currentAmmo = 0, int range = 0)
        {
            Name = name;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            AmmoPerClip = ammoPerClip;
            CurrentAmmo = currentAmmo;
        }

        [Export]
        public string Name { get; private set; }

        [Export]
        public int MinDamage { get; private set; }

        [Export]
        public int MaxDamage { get; private set; }

        [Export]
        public int AmmoPerClip { get; private set; }

        [Export]
        public int Range { get; private set; }

        public int CurrentAmmo { get; private set; }

        public bool CanAttack => IsMelee || HaveAmmoLeft;

        public bool HaveAmmoLeft => CurrentAmmo > 0;

        public bool IsMelee => Range == 1;
        public bool UsesAmmo => AmmoPerClip > 0;

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

        private void DecreaseAmmo() => CurrentAmmo--;

        private void SetAmmo(int ammo) => CurrentAmmo = ammo;

    }
}
