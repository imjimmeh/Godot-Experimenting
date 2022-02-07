using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;


namespace FaffLatest.scripts.state
{
	public class SpawnManager : Node
	{
		private PackedScene playerCharacter;

		private Node aiCharactersRoot;
		private AStarNavigator astarNavigator;
		private Node inputManager;
		private Node playerCharactersRoot;

		public override void _Ready()
		{
			FindNeededNodes();
			Preload();

			base._Ready();

			var characters = new CharacterCreationStats[]
			{
				new CharacterCreationStats(new Vector3(1, 1, 1), "Dave", 100, true, 10.0f),
				new CharacterCreationStats(new Vector3(10, 1, 5), "NotDave", 100, true, 20.0f),
				new CharacterCreationStats(new Vector3(3, 1, 3), "Bob", 100, true, 5.0f),
				new CharacterCreationStats(new Vector3(17, 1, 3), "Bob", 100, true, 12.0f)
			};

			SpawnCharacters(characters);
		}

		public void SpawnCharacters(CharacterCreationStats[] charactersToCreate)
        {
			for(var x = 0; x < charactersToCreate.Length; x++)
            {
				SpawnPlayerCharacter(charactersToCreate[x]);
            }
        }

		public void SpawnPlayerCharacter(CharacterCreationStats stats)
        {
            var newCharacter = playerCharacter.Instance<Character>();
			playerCharactersRoot.AddChild(newCharacter);

            newCharacter.Stats.CharacterName = stats.Name;
            newCharacter.Stats.SetPlayerCharacter(stats.IsPlayerCharacter);
			newCharacter.Stats.MovementDistance = stats.MovementDistance;

            var newCharacterKinematicBody = newCharacter.GetNode<KinematicBody>("KinematicBody");
            newCharacterKinematicBody.Transform = new Transform(newCharacterKinematicBody.Transform.basis, stats.StartPosition);
			AddCharacterSignals(newCharacterKinematicBody);

			astarNavigator._On_Character_Created(newCharacter);
		}

		private void AddCharacterSignals(Node newCharacterKinematicBody)
		{
			newCharacterKinematicBody.Connect(SignalNames.Characters.CLICKED_ON, inputManager, SignalNames.Characters.CLICKED_ON_METHOD);
			inputManager.Connect(SignalNames.Characters.MOVE_TO, newCharacterKinematicBody, "_On_MoveTo");
			newCharacterKinematicBody.Connect("_Character_FinishedMoving", astarNavigator, "_On_Character_FinishedMoving");
		}

		private void FindNeededNodes()
		{
			aiCharactersRoot = GetNode<Node>("/root/Root/Characters/AI");
			astarNavigator = GetNode<AStarNavigator>("../AStarNavigator");
			playerCharactersRoot = GetNode<Node>("/root/Root/Characters/Player");
			inputManager = GetNode<Node>("../InputManager");
		}

		private void Preload()
		{
			playerCharacter = GD.Load<PackedScene>("res://scenes/characters/PlayerCharacter.tscn");
		}
	}
}
