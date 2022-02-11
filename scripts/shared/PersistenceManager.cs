using FaffLatest.scripts.characters;
using FaffLatest.scripts.map;
using Godot;

namespace FaffLatest.scripts.shared
{
	public class PersistenceManager : Node
	{
		[Export]
		public PackedScene TownScene { get; private set; }

		public override void _Ready()
		{
			base._Ready();
		}

		public MapInfo GetTownScene()
		{
			var characters = GetTree().GetNodesInGroup("playerCharacters");

			var asArray = new CharacterStats[characters.Count];

			for (var i = 0; i < characters.Count; i++)
			{
				var character = ((Character)characters[i]).Stats;

				if(character.IsPlayerCharacter)
                {
					asArray[i] = character;
                }
			}

			return new MapInfo(TownScene, asArray);
		}
	}
}
