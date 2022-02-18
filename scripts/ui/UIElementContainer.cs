using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;
using System;
using System.Text;

namespace FaffLatest.scripts.ui
{ 
	public class UIElementContainer : Control
	{
		[Export]
		public PackedScene CharacterDamageTextScene { get; private set; }

		[Export]
		public DynamicFont CharacterDamageTextFont { get; private set; }

		[Export]
		public PackedScene LabelWithTimeToLiveScene { get; private set; }

		[Export]
		public DynamicFont LabelWithTimeToLiveFont { get; private set; }

        private const int BIG_FONT_SIZE = 32;
        private const int DAMAGE_FONT_SIZE = 20;

        private Camera camera;

		public CharacterSelector CharacterSelector { get; private set; }

		public SelectedCharacter SelectedCharacter { get; private set; }

		public HealthBar HealthBar { get; private set; }

		public Control EffectLabelsParent { get; private set; }

		public ConfirmationDialog ConfirmationDialog {get; private set;}

		public override void _Ready()
		{
			GetUIChildrenNodes();
			ConnectSignals();

			var viewportSize = GetViewport().GetVisibleRect().Size;
			SelectedCharacter.RectPosition = new Vector2(viewportSize.x - 200, viewportSize.y - 250);

			CharacterDamageTextFont.Size = DAMAGE_FONT_SIZE;
			CharacterDamageTextFont.OutlineSize = 1;
			CharacterDamageTextFont.OutlineColor = new Color(0, 0, 0, 1);
			CharacterDamageTextFont.UseFilter = true;

			LabelWithTimeToLiveFont.Size = BIG_FONT_SIZE;
			LabelWithTimeToLiveFont.OutlineSize = 3;
			LabelWithTimeToLiveFont.OutlineColor = new Color(0, 0, 0, 1);
			LabelWithTimeToLiveFont.UseFilter = true;

			base._Ready();
		}

		private void GetUIChildrenNodes()
		{
			CharacterSelector = GetNode<CharacterSelector>("CharacterSelector");
			ConfirmationDialog = GetNode<ConfirmationDialog>("ConfirmationDialog");
			SelectedCharacter = GetNode<SelectedCharacter>("SelectedCharacter");
			EffectLabelsParent = GetNode<Control>("EffectLabels");
		}

		private void ConnectSignals()
		{
			SelectedCharacter.ConnectSignals(GetNode(NodeReferences.Systems.GAMESTATE_MANAGER));
		}

		private void _On_Confirm_Character_Attack(Character character)
		{
			var dialogTextBuilder = new StringBuilder();
			dialogTextBuilder.AppendLine($"{character.Stats.CharacterName} still has {character.ProperBody.MovementStats.AmountLeftToMoveThisTurn} moves left this turn");
			dialogTextBuilder.AppendLine("If you attack you will be unable to move for the rest of the turn - are you sure?");

			ConfirmationDialog.DialogText = dialogTextBuilder.ToString();

			ConfirmationDialog.WindowTitle = "Confirm Attack";
			ConfirmationDialog.Show();
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
			var parent = CharacterDamageTextScene.Instance();
			var centererNode = parent.GetNode<WorldCenteredControl>("WorldCenteredControl");

			centererNode.Initialise(parent as Control, position, camera);

			EffectLabelsParent.AddChild(parent);

			var label = parent.GetNode<Label>("Label");
			label.AddFontOverride("font", CharacterDamageTextFont);
			label.Text = text;
			label.AddColorOverride("font_color", Colors.Red);

			return label;
		}

		public Control SpawnBigTemporaryText(Vector2 position, string text)
		{
			var label = LabelWithTimeToLiveScene.Instance<Label>();

			var blankNode = new Node();

			AddChild(blankNode);
			blankNode.AddChild(label);

			label.AddFontOverride("font", LabelWithTimeToLiveFont);
			label.Text = text;
			label.AddColorOverride("font_color", Colors.Red);
			label.RectPosition = new Vector2(position.x - ((6 * BIG_FONT_SIZE) / 2), position.y);

			return label;
		}
	}
}
