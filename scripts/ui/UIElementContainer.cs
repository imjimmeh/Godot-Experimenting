using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;
using System;

namespace FaffLatest.scripts.ui
{ 
	public class UIElementContainer : Node
	{
		private Camera camera;

		public CharacterSelector CharacterSelector { get; private set; }

		public SelectedCharacter SelectedCharacter { get; private set; }

		public HealthBar HealthBar { get; private set; }

		public Control EffectLabelsParent { get; private set; }

		public override void _Ready()
        {
            GetUIChildrenNodes();
			ConnectSignals();

            var viewportSize = GetViewport().GetVisibleRect().Size;
            SelectedCharacter.RectPosition = new Vector2(viewportSize.x - 200, viewportSize.y - 200);


            base._Ready();
        }

        private void GetUIChildrenNodes()
        {
			GD.Print("Getting children nodes");
            CharacterSelector = GetNode<CharacterSelector>("CharacterSelector");
            SelectedCharacter = GetNode<SelectedCharacter>("SelectedCharacter");
            EffectLabelsParent = GetNode<Control>("EffectLabels");
        }

		private void ConnectSignals()
        {
			SelectedCharacter.ConnectSignals(GetNode(NodeReferences.Systems.GAMESTATE_MANAGER));
		}

		private void _On_Character_ReceivedDamage(Node character, int damage, Node origin, bool killingBlow)
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
