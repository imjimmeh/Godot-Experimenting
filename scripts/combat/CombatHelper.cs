using FaffLatest.scripts.characters;
using System;
using Godot;

namespace FaffLatest.scripts.combat
{
    public static class CombatHelper
    {
        public static bool TryIssueAttackCommand(this Character attacker, Character receiver)
        {
            if(!IsWithinRange(attacker.ProperBody.Transform.origin, receiver.ProperBody.Transform.origin, attacker.Stats.EquippedWeapon.Range))
            {
                GD.Print($"Out of range");
                return false;
            }

            receiver._On_Character_ReceiveDamage(receiver.Stats.EquippedWeapon.MinDamage, attacker);
            return true;
        }

        private static bool IsWithinRange(Vector3 origin, Vector3 target, int maxRange) => (target - origin).Length() <= maxRange;

    }
}
