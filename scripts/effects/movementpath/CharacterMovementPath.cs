using FaffLatest.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.shared;
using Godot;

namespace FaffLatest.scripts.effects
{
    public class CharacterMovementPath : CharacterOwnedMeshInstance
	{
        public CharacterMovementPath()
        {

        }

        private void _On_Character_ReachedPathPart(Node character, Vector3 part)
        {
            if (!ShouldHidePart(character, part))
                return;

            DisconnectAndRemove();
        }

        public void DisconnectAndRemove()
        {
            DisconnectSignals();
            RemoveFromScene();
        }

        private void DisconnectSignals()
        {
            characterBody.GetNode("PathMover").Disconnect(SignalNames.Characters.REACHED_PATH_PART, this, SignalNames.Characters.REACHED_PATH_PART_METHOD);
        }

        private bool ShouldHidePart(Node character, Vector3 pathLocation)
        {
			if (character == null)
				return false;

            var pathOnSameY = pathLocation.CopyYValue(Transform.origin);

            var isThisPath = (pathOnSameY - Transform.origin).Length() == 0.0f;

            return isThisPath;
        }
    }
}