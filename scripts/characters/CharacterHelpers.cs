using FaffLatest.scripts.shared;

namespace FaffLatest.scripts.characters
{
    public static class CharacterHelpers{
        public static float DistanceToIgnoringHeight(this Character origin, Character target) 
            => origin.ProperBody.GlobalTransform.origin.DistanceToIgnoringHeight(target.ProperBody.GlobalTransform.origin);
    }
}