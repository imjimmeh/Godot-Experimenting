using FaffLatest.scripts.shared;

namespace FaffLatest.scripts.characters
{
    public static class CharacterHelpers{
        public static float DistanceToIgnoringHeight(this Character origin, Character target) 
            => origin.Body.GlobalTransform.origin.DistanceToIgnoringHeight(target.Body.GlobalTransform.origin);
    }
}