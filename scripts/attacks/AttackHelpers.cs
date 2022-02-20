using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.state;
using FaffLatest.scripts.ui;

namespace FaffLatest.scripts.attacks
{
    public static class AttackHelpers
    {
        private const string OUT_OF_ATTACKS = "Out of attacks!";
        private const string OUT_OF_RANGE = "Out of range!";

        public static async Task<bool> TryAttack(Character attacker, Character target)
        {
            var targetCheck = attacker.CanAttackTarget(target);
            var canAttack = ProceedWithAttack(target, targetCheck);

				if(!canAttack)
					return false;

                if (attacker.Stats.IsPlayerCharacter && attacker.Body.MovementStats.AmountLeftToMoveThisTurn > 0)
                {
                    var forfeitMovement = await ConfirmMovementForfeit(attacker);

                    if(!forfeitMovement)
                        return false;

                    attacker.Body.MovementStats.EndMovementTurn();
                }

                var attackResult = attacker.TryAttackTarget(target);
                return attackResult == weapons.AttackResult.Success;
        }

        
        private static async Task<bool> ConfirmMovementForfeit(Character character)
        {
            var proceed = await UIManager.Instance.ConfirmCharacterAttack(character);

            if (!proceed)
            {
                UIManager.Instance.SpawnDamageLabel(character.Body.GlobalTransform.origin, "Cancelling!");
            }

            return proceed;
        }

        private static bool ProceedWithAttack(Character character, weapons.AttackResult canAttackTarget)
        {
            switch (canAttackTarget)
            {
                case weapons.AttackResult.OutOfRange:
                    {
                        ShowPCFeedback(character, OUT_OF_RANGE);
                        return false;
                    }

                case weapons.AttackResult.OutOfAttacksForTurn:
                    {
                        ShowPCFeedback(character, OUT_OF_ATTACKS);
                        return false;
                    }
            }

			return true;
        }

        private static void ShowPCFeedback(Character character, string message)
        {
            if (!character.Stats.IsPlayerCharacter)
                UIManager.Instance.SpawnDamageLabel(character.Body.GlobalTransform.origin, message);
        }
    }
}