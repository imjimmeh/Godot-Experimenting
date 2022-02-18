using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.shared;
using Godot;

namespace FaffLatest.scripts.ui{
    public class MiniCharacterHealthBar : CharacterValueBinderNode
    {
        private Camera camera;
        private HealthBar healthBar;
        private WorldCenteredControl worldCenterer;

        private Vector3 charactersLastPosition;
        private int characterLastHealth = 0;

        public override async void _Ready()
        {
            base._Ready();

            parent = GetParent().GetParent().GetParent<Character>();
            await ToSignal(parent, "ready");
            _On_Character_Ready(parent);
        }

        public override void _Process(float delta)
        {
            bool characterHasMoved = charactersLastPosition != parent.Body.GlobalTransform.origin;

            if (characterHasMoved)
            {
                worldCenterer.SetPosition(PositionToDisplay);
                charactersLastPosition = parent.Body.GlobalTransform.origin;
            }
        }

        private void _On_Character_Ready(Character character)
        {
            healthBar = GetParent<HealthBar>();
            SetParentAndAction(character, healthBar.SetValuesToCharacterValues);
            GetCamera();

            character.Connect(nameof(Character._Character_ReceivedDamage), this, SignalNames.Characters.RECEIVED_DAMAGE_METHOD);
            
            worldCenterer = healthBar.GetNode<WorldCenteredControl>("WorldCenteredControl");

            worldCenterer.Initialise(healthBar, PositionToDisplay, camera);
            charactersLastPosition = character.Body.GlobalTransform.origin;
            healthBar.RectSize = new Vector2(80, 20);
            
            healthBar.CallDeferred("show");

            _On_Character_ReceivedDamage(parent, 0, null, false);
        }
        
        private Vector3 PositionToDisplay => parent.Body.GlobalTransform.origin;

        private void _On_Character_ReceivedDamage(Node character, int damage, Node origin, bool killingBlow)
        {
            if (camera == null)
                GetCamera();

            UpdateValues();
        }

        private void GetCamera()
        {
            camera = GetNode<Camera>(NodeReferences.BaseLevel.Cameras.MAIN_CAMERA);
        }
    }
}