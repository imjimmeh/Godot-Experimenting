using FaffLatest.scripts.characters;
using FaffLatest.scripts.state;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            AddCharacters(spawnManager.Characters);
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

        private static MiniCharacterDisplay CreateDisplay(Character character, PackedScene miniDisplay)
        {
            var newInstance = miniDisplay.Instance() as MiniCharacterDisplay;
            newInstance.SetCharacter(character);

            return newInstance;
        }
    }
}
