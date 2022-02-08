using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.effects;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.ui;
using Godot;
using System;

namespace FaffLatest.scripts.state
{
	public class SpawnManager : Node
	{
		private PackedScene characterBase;

		private Node aiCharactersRoot;
		private AStarNavigator astarNavigator;
		private Node inputManager;
		private Node playerCharactersRoot;
		private Node characterMovementPathManager;

		private UIElementContainer ui;

		private Character[] characters;

        public Character[] Characters { get => characters; set => characters = value; }

        public override void _Ready()
		{
			FindNeededNodes();
			Preload();

			base._Ready();

			var characters = new CharacterCreationStats[]
			{
				new CharacterCreationStats(new Vector3(1, 1, 1), "Dave", 100, true, 10),
				new CharacterCreationStats(new Vector3(10, 1, 5), "NotDave", 100, true, 20),
				new CharacterCreationStats(new Vector3(3, 1, 3), "Bob", 100, true, 5),
				new CharacterCreationStats(new Vector3(17, 1, 3), "Bob", 100, true, 12),
				new CharacterCreationStats(new Vector3(5, 1, 5), "Bad man", 100, false, 12),
				new CharacterCreationStats(new Vector3(5, 1, 7), "Bad man2", 100, false, 12)
			};

			SpawnCharacters(characters);
		}

		public void SpawnCharacters(CharacterCreationStats[] charactersToCreate)
        {
			Character[] characters = new Character[charactersToCreate.Length];

			int pc = 0;

			for(var x = 0; x < charactersToCreate.Length; x++)
            {
				var character = SpawnCharacter(charactersToCreate[x]);

				if(character.Stats.IsPlayerCharacter)
                {
					characters[pc] = character;
					pc++;
                }

			}

			Array.Resize(ref characters, pc);

			this.characters = characters;
        }

		public Character SpawnCharacter(CharacterCreationStats stats)
        {
            var newCharacter = characterBase.Instance<Character>();

            newCharacter.Stats.CharacterName = stats.Name;
			newCharacter.Stats.IsPlayerCharacter = stats.IsPlayerCharacter;
			newCharacter.Stats.SetMaxMovementDistance(stats.MovementDistance);

            var newCharacterKinematicBody = newCharacter.GetNode<KinematicBody>("KinematicBody");
            newCharacterKinematicBody.Transform = new Transform(newCharacterKinematicBody.Transform.basis, stats.StartPosition);

			if (stats.IsPlayerCharacter)
				playerCharactersRoot.AddChild(newCharacter);
			else
				aiCharactersRoot.AddChild(newCharacter);

			AddCharacterSignals(newCharacterKinematicBody);

			astarNavigator._On_Character_Created(newCharacter);

			return newCharacter;
		}

		private void AddCharacterSignals(Node newCharacterKinematicBody)
		{
			inputManager.Connect(SignalNames.Characters.MOVE_TO, newCharacterKinematicBody, "_On_Character_MoveTo");

			newCharacterKinematicBody.Connect(SignalNames.Characters.CLICKED_ON, inputManager, SignalNames.Characters.CLICKED_ON_METHOD);
			newCharacterKinematicBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, astarNavigator, SignalNames.Characters.MOVEMENT_FINISHED_METHOD);
			newCharacterKinematicBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, characterMovementPathManager, SignalNames.Characters.MOVEMENT_FINISHED_METHOD);
			newCharacterKinematicBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, GetNode("../GameStateManager"), SignalNames.Characters.MOVEMENT_FINISHED_METHOD);
		}

		private void FindNeededNodes()
		{
			aiCharactersRoot = GetNode<Node>("/root/Root/Characters/AI");
			astarNavigator = GetNode<AStarNavigator>("../AStarNavigator");
			characterMovementPathManager = GetNode<CharacterMovementPathManager>("/root/Root/Effects/CharacterMovementPath");
			playerCharactersRoot = GetNode<Node>("/root/Root/Characters/Player");
			inputManager = GetNode<Node>("../InputManager");
			ui = GetNode<UIElementContainer>("/root/Root/UI");
		}

		private void Preload()
		{
			characterBase = GD.Load<PackedScene>("res://scenes/characters/Character.tscn");
		}
	}
}