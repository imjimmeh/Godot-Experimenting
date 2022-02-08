using FaffLatest.characters;
using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.effects
{
    public class CharacterMovementPath : CharacterOwnedMeshInstance
	{
        public CharacterMovementPath()
        {

        }

        private void _On_Character_ReachedPathPart(Node character, Vector3 pathLocation)
        {
            if (!ShouldHidePart(character, pathLocation))
                return;

            HideFromSceneAndDisconnect();
            DisconnectSignals();
        }

        private void DisconnectSignals()
        {
            characterBody.Disconnect(SignalNames.Characters.REACHED_PATH_PART, this, SignalNames.Characters.REACHED_PATH_PART_METHOD);
        }

        private bool ShouldHidePart(Node character, Vector3 pathLocation)
        {
			if (character == null)
				return false;

			var isThisPath = pathLocation.IsEqualApprox(Transform.origin);

			return isThisPath;
        }
    }
}