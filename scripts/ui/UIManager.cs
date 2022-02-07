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

		private void _On_Turn_Change(string whoseTurn)
        {
			var newLabel = new LabelWithTimeToLive();
			newLabel.Text = $"{whoseTurn} Turn";
			var viewportSize = GetViewport().GetVisibleRect().Size;

			newLabel.RectPosition = new Vector2(viewportSize.x * 0.5f, viewportSize.y * 0.5f);
			elementContainer.AddChild(newLabel);


			//GD.Print("Created turn change text");
		}
	}

}
