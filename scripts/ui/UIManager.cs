using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.ui
{

    public class UIManager : Node
	{
		private UIElementContainer elementContainer;

		public override void _Ready()
        {
            base._Ready();

            FindChildren();
		}

		private void FindChildren()
        {
            elementContainer = GetNode<UIElementContainer>("/root/Root/UI");
        }

        private void RegisterSignals()
		{

		}

		private void _On_Character_Selected(Character character)
		{
			elementContainer.CharacterFaceIcon.Texture = character.Stats.FaceIcon;
		}

		private void _On_Character_Unselected()
		{
			elementContainer.CharacterFaceIcon.Texture = null;
		}
	}

}
