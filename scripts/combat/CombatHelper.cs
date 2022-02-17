using FaffLatest.scripts.characters;
using System;
using Godot;

namespace FaffLatest.scripts.combat
{
    public static class CombatHelper
    {
        public static bool TryIssueAttackCommand(this Character attacker, Character receiver)
        {
            if (!IsWithinRange(attacker.ProperBody.GlobalTransform.origin, receiver.ProperBody.GlobalTransform.origin, attacker.Stats.EquippedWeapon.Range))
            {
                return false;
            }

            var canAttack = attacker.Stats.EquippedWeapon.TryAttack(out int damage);

            if(!canAttack)
                return false;

            receiver._On_Character_ReceiveDamage(damage, attacker);
            return true;
        }

        private static bool IsWithinRange(Vector3 origin, Vector3 target, int maxRange) => (target - origin).Length() <= maxRange;

    }
}
