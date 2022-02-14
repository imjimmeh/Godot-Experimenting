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
                GD.Print($"Out of range - we are at {attacker.ProperBody.Transform.origin}, receiver is at {receiver.ProperBody.GlobalTransform.origin}, our weapon range is {attacker.Stats.EquippedWeapon.Range}");
                return false;
            }

            receiver._On_Character_ReceiveDamage(receiver.Stats.EquippedWeapon.MinDamage, attacker);
            return true;
        }

        private static bool IsWithinRange(Vector3 origin, Vector3 target, int maxRange) => (target - origin).Length() <= maxRange;

    }
}
