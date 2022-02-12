using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;
using System;

namespace FaffLatest.scripts.ui
{ 
	public class UIElementContainer : Node
	{
		private Camera camera;

		private Control selectedCharacterPart;

		private TextureRect characterFaceIcon;
		public TextureRect CharacterFaceIcon {get =>characterFaceIcon; private set => characterFaceIcon = value;}

		public Label CharacterName;

		public CharacterSelector CharacterSelector { get; private set; }

		public HealthBar HealthBar { get; private set; }

		public Control EffectLabelsParent { get; private set; }

		public override void _Ready()
		{
			selectedCharacterPart = GetNode<Control>("SelectedCharacter");

			var viewportSize = GetViewport().GetVisibleRect().Size;

			selectedCharacterPart.RectPosition = new Vector2(viewportSize.x - 200, viewportSize.y - 200);

			CharacterFaceIcon = selectedCharacterPart.	GetNode<TextureRect>("CharacterIcon");
			CharacterName = CharacterFaceIcon.GetNode<Label>("Name");

			CharacterSelector = GetNode<CharacterSelector>("CharacterSelector");
			HealthBar = CharacterFaceIcon.GetNode<HealthBar>("HealthBar");

			EffectLabelsParent = GetNode<Control>("EffectLabels");

			base._Ready();
		}

		private void _On_Character_ReceivedDamage(Node character, int damage, Node origin)
		{
			var asChar = character as Character;

			if(camera == null)
				camera = GetNode<Camera>(NodeReferences.BaseLevel.Cameras.MAIN_CAMERA);

			SpawnExpiringLabel(camera.UnprojectPosition(asChar.ProperBody.GlobalTransform.origin), damage.ToString(), 5, true);
		}

		private LabelWithTimeToLive SpawnExpiringLabel(Vector2 pos, string text, float timeToLive, bool addChild = true)
		{
			var label = new LabelWithTimeToLive(timeToLive);
			label.Text = text;
			label.RectPosition = pos;
			
			if(addChild)
				EffectLabelsParent.AddChild(label);

			return label;
		}
	}
}
