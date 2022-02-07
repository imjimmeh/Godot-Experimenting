using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;
using System;

namespace FaffLatest.scripts.effects
{
	public class CharacterMovementPath : MeshInstance
	{
        private bool isHidden = true;

		private Node parentNode;
        private Node characterBody;

        public CharacterMovementPath()
        {

        }

        public void SetParentNode(Node parent) => parentNode = parent;
        public void SetCharacterBody(Node characterBody) => this.characterBody = characterBody;

        private void _On_Character_ReachedPathPart(Node character, Vector3 pathLocation)
        {
            if (!ShouldHidePart(character, pathLocation))
                return;

            GD.Print($"Disposing part");
            HideFromSceneAndDisconnect();
        }

        private void HideFromSceneAndDisconnect()
        {
            parentNode.RemoveChild(this);
            characterBody.Disconnect(SignalNames.Characters.REACHED_PATH_PART, this, SignalNames.Characters.REACHED_PATH_PART_METHOD);
            isHidden = true;
        }

        public void DisconnectAndDispose()
        {
            if(!isHidden)
            {
                HideFromSceneAndDisconnect();
            }

            Dispose();
        }

        private bool ShouldHidePart(Node character, Vector3 pathLocation)
        {
			if (character == null)
				return false;

			var isThisPath = pathLocation.IsEqualApprox(Transform.origin);

			return isThisPath;
        }

        public override void _Ready()
        {
            base._Ready();

            isHidden = false;
        }
    }
}