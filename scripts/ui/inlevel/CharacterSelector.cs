using System.Collections.Generic;
using System.Linq;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
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
			GetNode(NodeReferences.Systems.SPAWN_MANAGER)
				.Connect(SignalNames.Loading.CHARACTERS_SPAWNED, this, SignalNames.Loading.CHARACTERS_SPAWNED_METHOD);
		}

		public void CreateDisplays()
		{
			var characters = CharacterManager.Instance.PlayerCharacters;

			if (characters == null)
				throw new System.Exception("Could not find player characters");

			int characterCount = characters.Count() + 1;
			characterDisplays = new MiniCharacterDisplay[characterCount];

			int spriteSize = 32;

			var displays = characters.Select((character, index) => CreateDisplay(character, spriteSize, index));

			characterDisplays = displays.ToArray();
		}

		private void _On_Characters_Spawned()
		{
			CreateDisplays();
		}
		

		private MiniCharacterDisplay CreateDisplay(Character character, int spriteSize, int currentIndex)
		{
			var display = MiniDisplay.Instance() as MiniCharacterDisplay;
			display.SetCharacter(character);
			display.MarginTop = CalculateMarginTop(Spacing, spriteSize, currentIndex);

			CallDeferred("add_child", display);

			return display;
		}

		private static float CalculateMarginTop(float spacing, int spriteSize, int index) => (spacing * index) + (spriteSize * index);
	}
}
