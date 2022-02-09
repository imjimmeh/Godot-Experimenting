using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.ui
{

	public class UIManager : Node
	{
		private UIElementContainer elementContainer;

		public UIElementContainer ElementContainer { get => elementContainer; private set => elementContainer = value; }

		public override void _Ready()
		{
			base._Ready();

			FindChildren();
		}

		private void FindChildren()
		{
			ElementContainer = GetNode<UIElementContainer>("/root/Root/UI");
		}

		private void RegisterSignals()
		{

		}

		private void _On_Character_Selected(Character character)
		{
			ElementContainer.CharacterFaceIcon.Texture = character.Stats.FaceIcon;
			ElementContainer.CharacterName.Text = character.Stats.CharacterName;

			ElementContainer.HealthBar.Character = character;
			ElementContainer.HealthBar.Show();
		}

		private void _On_Character_Unselected()
		{
			ElementContainer.CharacterFaceIcon.Texture = null;
			ElementContainer.CharacterName.Text = "";

			ElementContainer.HealthBar.Character = null;
			ElementContainer.HealthBar.Hide();
		}

		private void _On_Turn_Change(string whoseTurn)
		{
			var newLabel = new LabelWithTimeToLive
			{
				Text = $"{whoseTurn} Turn"
			};

			var viewportSize = GetViewport().GetVisibleRect().Size;

			newLabel.RectPosition = new Vector2(viewportSize.x * 0.5f, viewportSize.y * 0.5f);
			ElementContainer.AddChild(newLabel);
		}
	}

}
