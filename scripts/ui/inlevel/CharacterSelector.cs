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

		public void AddCharacters(IEnumerable<Character> characters)
		{
			if (characters == null)
				return;

			int characterCount = characters.Count();
			characterDisplays = new MiniCharacterDisplay[characterCount];

			var index = 0;

			int spriteSize = 32;

			foreach(var character in characters)
			{
				var display = CreateDisplay(character, spriteSize, index);
				characterDisplays[characterCount] = display;
				CallDeferred("add_child",display);
				index++;
			}
		}

		private void _On_Characters_Spawned()
		{
			AddCharacters(CharacterManager.Instance.PlayerCharacters);
		}
		

		private MiniCharacterDisplay CreateDisplay(Character character, int spriteSize, int currentIndex)
		{
			var newInstance = MiniDisplay.Instance() as MiniCharacterDisplay;
			newInstance.SetCharacter(character);
			newInstance.MarginTop = CalculateMarginTop(Spacing, spriteSize, currentIndex);

			return newInstance;
		}

		private static float CalculateMarginTop(float spacing, int spriteSize, int index) => (spacing * index) + (spriteSize * index);
	}
}
