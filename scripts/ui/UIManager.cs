using System.Text;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;
using static FaffLatest.scripts.shared.ViewportHelpers;

namespace FaffLatest.scripts.ui
{

	public class UIManager : Node
	{
        private const int BIG_FONT_SIZE = 32;
        private const int DAMAGE_FONT_SIZE = 20;

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
            GenerateNewTurnText(whoseTurn);
        }

        private void GenerateNewTurnText(string whoseTurn)
        {
            var screenCenter = this.GetCenterOfScreen();
            var (parent, label) = UiLabelFactory.GenerateUiLabel($"{whoseTurn}" + "\n" + "Turn", new FontValues(Colors.Red, 32, Colors.Black, 3), null, null, 3);
            var labelSize = whoseTurn.Length * 16;
            parent.RectPosition = new Vector2(screenCenter.x - labelSize, screenCenter.y - 32);
            label.Align = Label.AlignEnum.Center;
        }

        public void ShowUi()
		{
			elementContainer.Show();
		}

		private void _On_Confirm_Character_Attack(Character character)
        {
            BuildConfirmationDialog(character);
            elementContainer.ConfirmationDialog.Show();
        }

        private void BuildConfirmationDialog(Character character)
        {
            var dialogTextBuilder = new StringBuilder();
            dialogTextBuilder.AppendLine($"{character.Stats.CharacterName} still has {character.ProperBody.MovementStats.AmountLeftToMoveThisTurn} moves left this turn");
            dialogTextBuilder.AppendLine("If you attack you will be unable to move for the rest of the turn - are you sure?");

            elementContainer.ConfirmationDialog.DialogText = dialogTextBuilder.ToString();

            elementContainer.ConfirmationDialog.WindowTitle = "Confirm Attack";
        }

        private void _On_Character_ReceivedDamage(Node character, int damage, Node origin, bool killingBlow)
		{
            var asChar = character as Character;

			if(camera == null)
				camera = GetNode<Camera>(NodeReferences.BaseLevel.Cameras.MAIN_CAMERA);

			SpawnDamageLabel(asChar.ProperBody.GlobalTransform.origin, damage.ToString());

			if (killingBlow)
            {
				SpawnDamageLabel(asChar.ProperBody.GlobalTransform.origin, "KILLED");
            }
		}

		public Control SpawnDamageLabel(Vector3 position, string text)
		{
            var (parent, label) = UiLabelFactory.GenerateUiLabel(text: text,
																values: new FontValues(Colors.Red, 16, Colors.Black, 1), 
																centerOnWorldPosition: position, 
																movementVector: new Vector2(0, -1), 
																timeToLive: 3);

			return label;
		}

		public Control SpawnBigTemporaryText(Vector2 position, string text)
		{
			var (parent, label) = UiLabelFactory.GenerateUiLabel(text: text,
																values: new FontValues(Colors.Red, 16, Colors.Black, 1), 
																centerOnWorldPosition: null, 
																movementVector: new Vector2(0, -1), 
																timeToLive: 3);

			label.RectPosition = new Vector2(position.x - ((6 * BIG_FONT_SIZE) / 2), position.y);

			return label;
		}
	}

}
