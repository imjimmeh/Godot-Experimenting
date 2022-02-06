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

		private GameStateManager gameStateManager;

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
            playerCharactersRoot.AddChild(newCharacter);

            newCharacter.Stats.CharacterName = name;
            newCharacter.Stats.SetPlayerCharacter(isPlayerCharacter);

            var newCharacterKinematicBody = newCharacter.GetNode<KinematicBody>("KinematicBody");
            newCharacterKinematicBody.Transform = new Transform(newCharacterKinematicBody.Transform.basis, position);

			AddCharacterSignals(newCharacterKinematicBody);
		}

		private void AddCharacterSignals(KinematicBody newCharacterKinematicBody)
        {
            newCharacterKinematicBody.Connect(SignalNames.Characters.CLICKED_ON, inputManager, SignalNames.Characters.CLICKED_ON_METHOD);
            inputManager.Connect(SignalNames.Characters.MOVE_TO, newCharacterKinematicBody, "_On_MoveTo");

			var movementHelper = newCharacterKinematicBody.GetNode<MovementDisplayMeshInstance>("MovementDisplayMeshInstance");
		}

		private void FindNeededNodes()
		{
			aiCharactersRoot = GetNode<Node>("/root/Root/Characters/AI");
			gameStateManager = GetNode<GameStateManager>("../GameStateManager");
			playerCharactersRoot = GetNode<Node>("/root/Root/Characters/Player");
			inputManager = GetNode<Node>("../InputManager");
		}

		private void Preload()
		{
			playerCharacter = GD.Load<PackedScene>("res://scenes/characters/PlayerCharacter.tscn");
		}
	}
}
