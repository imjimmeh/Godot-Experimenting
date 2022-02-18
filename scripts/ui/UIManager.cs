using System;
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
        [Signal]
        public delegate void _ConfirmationDialog_Response(bool ok);

        public static UIManager Instance;

        private const int BIG_FONT_SIZE = 32;
        private const int DAMAGE_FONT_SIZE = 20;

		private Camera camera;
		private FontManager fontManager;
		private UIElementContainer elementContainer;

		public UIElementContainer ElementContainer { get => elementContainer; private set => elementContainer = value; }

		[Export]
		public DynamicFontData FontToUse { get; private set; }

		public FontManager Fonts => fontManager;
		
        private bool confirmationDialogResponse = false;
        private bool signalsConnected = false;
        
		public override async void _Ready()
        {
            FindChildren();
            fontManager = new FontManager(FontToUse);

            base._Ready();

			await GetCamera();
            InitialiseFactories();

            Instance = this;
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

        private void ConfirmDialog_OK_Pressed()
        {
            confirmationDialogResponse = true;
            EmitSignal(nameof(_ConfirmationDialog_Response), true);
        }

           private void ConfirmDialog_Cancel_Pressed()
        {
            confirmationDialogResponse = false;
            EmitSignal(nameof(_ConfirmationDialog_Response), false);
        }

        private void GenerateNewTurnText(string whoseTurn)
        {   
            var text = ($"{whoseTurn}" + "\n" + "Turn");
            var labelSize = whoseTurn.Length * 16;

            var screenCenter = this.GetCenterOfScreen();
            var position = new Vector2(screenCenter.x - labelSize, screenCenter.y - 32);

            var (parent, label) = UiLabelFactory.GenerateUiLabel(text: text,
                                                                values: new FontValues(Colors.Red, 32, Colors.Black, 3),
                                                                position: position,
                                                                centerOnWorldPosition: null, 
                                                                movementVector: null, 
                                                                timeToLive: 2);
            label.Align = Label.AlignEnum.Center;
        }

        public void ShowUi()
		{
			elementContainer.Show();
		}

        public Control CreateLabelAtWorldPosition(string text, FontValues fontValues, Vector3 worldPosition)
        {
            var (parent, label) = UiLabelFactory.GenerateUiLabel(text,
                                                    new FontValues(Colors.White, 16, Colors.Black, 1),
                                                    camera.UnprojectPosition(worldPosition),
                                                    worldPosition);

            return parent;
        }
        
		public async Task<bool> ConfirmCharacterAttack(Character character)
        {
            var dialog = BuildConfirmationDialog(character);
            var okButton = dialog.GetOk();
            var cancelButton = dialog.GetCancel();

            elementContainer.ConfirmationDialog.CallDeferred("show");
            
            await ToSignal(this, nameof(_ConfirmationDialog_Response));
            return confirmationDialogResponse;
        }

        private ConfirmationDialog BuildConfirmationDialog(Character character)
        {
            var dialogTextBuilder = new StringBuilder();
            dialogTextBuilder.AppendLine($"{character.Stats.CharacterName} still has {character.Body.MovementStats.AmountLeftToMoveThisTurn} moves left this turn");
            dialogTextBuilder.AppendLine("If you attack you will be unable to move for the rest of the turn - are you sure?");
            elementContainer.ConfirmationDialog.DialogText = dialogTextBuilder.ToString();
            elementContainer.ConfirmationDialog.WindowTitle = "Confirm Attack";
            elementContainer.ConfirmationDialog.Popup_(new Rect2(GetViewport().GetCenterOfScreen(), GetViewport().GetCenterOfScreen()/2));

            if(!signalsConnected)
            {
                elementContainer.ConfirmationDialog.GetOk().Connect("pressed", this, nameof(ConfirmDialog_OK_Pressed));
                elementContainer.ConfirmationDialog.GetCancel().Connect("pressed", this, nameof(ConfirmDialog_Cancel_Pressed));
                signalsConnected = true;
            }

            return elementContainer.ConfirmationDialog;
        }

        private void _On_Character_ReceivedDamage(Node character, int damage, Node origin, bool killingBlow)
		{
            var asChar = character as Character;

			if(camera == null)
				camera = GetNode<Camera>(NodeReferences.BaseLevel.Cameras.MAIN_CAMERA);

			SpawnDamageLabel(asChar.Body.GlobalTransform.origin, damage.ToString());
		}

		public Control SpawnDamageLabel(Vector3 position, string text)
		{
            var (parent, label) = UiLabelFactory.GenerateUiLabel(text: text,
																values: new FontValues(Colors.Red, 16, Colors.Black, 1),
                                                                position: camera.UnprojectPosition(position),
																centerOnWorldPosition: position, 
																movementVector: new Vector2(0, -1), 
																timeToLive: 3);

			return label;
		}

		public Control SpawnBigTemporaryText(Vector2 position, string text)
		{
            position = new Vector2(position.x - ((6 * BIG_FONT_SIZE) / 2), position.y);

			var (parent, label) = UiLabelFactory.GenerateUiLabel(text: text,
																values: new FontValues(Colors.Red, 16, Colors.Black, 1), 
                                                                position: position,
																centerOnWorldPosition: null, 
																movementVector: new Vector2(0, -1), 
																timeToLive: 3);

			return label;
		}
	}

}
