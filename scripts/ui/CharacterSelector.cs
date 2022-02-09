using FaffLatest.scripts.characters;
using FaffLatest.scripts.state;
using Godot;

namespace FaffLatest.scripts.ui
{
    public class CharacterSelector : Control
    {
        [Export]
        public PackedScene MiniDisplay { get; set; }

        [Export]
        public Vector2 StartPosition { get; set; } = new Vector2(10, 10);

        [Export]
        public float Spacing { get; set; }

        private MiniCharacterDisplay[] characterDisplays;

        public override void _Ready()
        {
            base._Ready();
            var spawnManager = GetNode<SpawnManager>("/root/Root/Systems/SpawnManager");

            spawnManager.Connect("_Characters_Spawned", this, "_Characters_Spawned");

        }

        public void AddCharacters(Character[] characters)
        {
            if (characters == null || characters.Length == 0)
                return;

            characterDisplays = new MiniCharacterDisplay[characters.Length];

            var drawn = 0;
            for (int i = 0; i < characters.Length; i++)
            {

                var display = CreateDisplay(characters[i], MiniDisplay);
                display.MarginTop = (Spacing * i) + (64 * i);
                AddChild(display);

                characterDisplays[i] = display;

                drawn++;
            }
        }

        private void _Characters_Spawned(Node spawnManager)
        {
            GD.Print(spawnManager);
            if(spawnManager is SpawnManager sm)
            {
                GD.Print(sm);

                AddCharacters(sm.Characters);
            }
        }

        private MiniCharacterDisplay CreateDisplay(Character character, PackedScene miniDisplay)
        {
            var newInstance = miniDisplay.Instance() as MiniCharacterDisplay;
            newInstance.SetCharacter(character);

            return newInstance;
        }
    }
}
