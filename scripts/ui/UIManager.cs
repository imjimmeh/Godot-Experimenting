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
			FindChildren();

			base._Ready();
		}

		private void FindChildren()
        {
            ElementContainer = GetNode<UIElementContainer>(NodeReferences.BaseLevel.UI);
		}


		private void _On_Turn_Change(string whoseTurn)
		{
			var viewportSize = GetViewport().GetVisibleRect().Size;

			var position = new Vector2(viewportSize.x * 0.5f, viewportSize.y * 0.5f);

			elementContainer.SpawnBigTemporaryText(position, $"{whoseTurn}" + "\n" + "Turn");
		}
	}

}
