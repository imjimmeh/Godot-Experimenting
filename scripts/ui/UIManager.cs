using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;

namespace FaffLatest.scripts.ui
{

	public class UIManager : Node
	{
		private Camera camera;
		private FontManager fontManager;
		private UIElementContainer elementContainer;

		public UIElementContainer ElementContainer { get => elementContainer; private set => elementContainer = value; }

		[Export]
		public DynamicFontData FontToUse { get; private set; }

		public FontManager Fonts => fontManager;
		
		public override async void _Ready()
        {
            FindChildren();
            fontManager = new FontManager(FontToUse);

            base._Ready();

			await GetCamera();
            InitialiseFactories();
        }

        private void InitialiseFactories()
        {
            UiLabelFactory.Initialise(ElementContainer, fontManager, camera);
        }

        private async Task GetCamera()
        {
            var camera = GetNode(NodeReferences.BaseLevel.Cameras.MAIN_CAMERA);
            await ToSignal(camera, "ready");
            this.camera = camera as Camera;
        }

        private void FindChildren()
        {
            ElementContainer = GetNode<UIElementContainer>(NodeReferences.BaseLevel.UI);
		}

		private void _On_Turn_Changed(string whoseTurn)
		{
			var viewportSize = GetViewport().GetVisibleRect().Size;

			var centerOfScreen = new Vector2(viewportSize.x * 0.5f, viewportSize.y * 0.5f);

			var (parent, label) = UiLabelFactory.GenerateUiLabel($"{whoseTurn}" + "\n" + "Turn", new FontValues(Colors.Red, 32, Colors.Black, 3), null, null, 3);



			var labelSize= whoseTurn.Length * 16;
			parent.RectPosition = new Vector2(centerOfScreen.x - labelSize, centerOfScreen.y - 32);

			label.Align = Label.AlignEnum.Center;
		}

		public void ShowUi()
		{
			elementContainer.Show();
		}
	}

}
