using FaffLatest.scripts.characters;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.ai
{
    public static class AiLogicHelpers
    {
        public static (Character closestCharacter, Vector3 targetPosition) GetNearestOpponentCharacter(this Character us)
        {
            Character closestCharacter = null;
            Vector3 targetPos = Vector3.Zero;

            float closestDistance = 99999;


            foreach (var character in AStarNavigator.Instance.CharacterLocations)
            {
                if (character.Key.Stats.IsPlayerCharacter == us.Stats.IsPlayerCharacter)
                    continue;

                (bool characterIsCloser, Vector3? target, float? result) 
                    = us.IsCloserThanCurrentTarget(character.Key, closestDistance);

                if(characterIsCloser)
                {
                    closestCharacter = character.Key;
                    targetPos = target.Value;
                    closestDistance = result.Value;
                }
            }

            return (closestCharacter, targetPos);
        }

        
        public static 
        (bool isCloser, Vector3? closestPosition, float? closestDistance) IsCloserThanCurrentTarget (this Character us,
                                                                                                          Character checking,
                                                                                                        float closestDistance)
        {
            var vector = (checking.Body.GlobalTransform.origin - us.Body.GlobalTransform.origin);
            var distance = vector.Length();

            if (distance > closestDistance)
                return (false, null, null);

            return (true, GetTargetPositionToMoveTo(checking, vector), distance);
        }

        private static Vector3 GetTargetPositionToMoveTo(Character character, Vector3 vector)
        {
            var direction = vector.Normalized().Round();

            return character.Body.GlobalTransform.origin - direction;;
        }
    }
}