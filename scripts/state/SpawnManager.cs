using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using Godot;


namespace FaffLatest.scripts.state
{
	public class SpawnManager : Node
	{
		private PackedScene playerCharacter;

		private Node aiCharactersRoot;
		private Node inputManager;
		private Node playerCharactersRoot;

		private CharacterStats characterStats;

		public override void _Ready()
		{
			base._Ready();

			FindNeededNodes();
			Preload();

			SpawnPlayerCharacter(new Vector3(1, 1, 1), "Dave", 100, true);
		}



		public void SpawnPlayerCharacter(Vector3 position, string name, int health, bool isPlayerCharacter)
		{
			var newCharacter = playerCharacter.Instance<Character>();

			newCharacter.SetPosition(position);
			playerCharactersRoot.AddChild(newCharacter);

			newCharacter.Stats.CharacterName = name;
			newCharacter.Stats.SetPlayerCharacter(isPlayerCharacter);

			var newCharacterKinematicBody = newCharacter.GetNode("KinematicBody");

			newCharacterKinematicBody.Connect(SignalNames.Characters.CLICKED_ON, inputManager, "_On_Character_ClickedOn");
			inputManager.Connect(SignalNames.Characters.MOVE_TO, newCharacterKinematicBody, "_On_MoveTo");
		}

		private void FindNeededNodes()
		{
			aiCharactersRoot = GetNode<Node>("/root/Root/Characters/AI");
			playerCharactersRoot = GetNode<Node>("/root/Root/Characters/Player");
			inputManager = GetNode<Node>("../InputManager");
		}

		private void Preload()
		{
			playerCharacter = GD.Load<PackedScene>("res://scenes/characters/PlayerCharacter.tscn");
		}
	}
}
