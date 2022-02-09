using FaffLatest.scripts.ai;
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
		[Signal]
		public delegate void _Characters_Spawned(Node spawnManager);

        private const string CHARACTERS_SPAWNED = "_Characters_Spawned";
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
		}

		public void SpawnCharacters(CharacterStats[] charactersToCreate, Vector3[] spawnPositions)
        {
			Character[] characters = new Character[charactersToCreate.Length];

			int pc = 0;
			int nonPc = 0;

			var aiChars = new Character[charactersToCreate.Length];
			for (var x = 0; x < charactersToCreate.Length; x++)
            {
				var character = SpawnCharacter(charactersToCreate[x], spawnPositions[x]);

				if(character.Stats.IsPlayerCharacter)
                {
					characters[pc] = character;
					pc++;
                }
                else
                {
					aiChars[nonPc] = character;
					nonPc++;
                }

			}

			Array.Resize(ref characters, pc);
			Array.Resize(ref aiChars, nonPc);

			this.characters = characters;

			var nodes = characters as Node[];

			GetNode<AIManager>("../AIManager").SetCharacters(aiChars);
			GD.Print($"{nodes.Length}");
			EmitSignal(CHARACTERS_SPAWNED, this);
        }

		public Character SpawnCharacter(CharacterStats stats, Vector3 spawnPosition)
        {
            var newCharacter = characterBase.Instance<Character>();

			newCharacter.Stats = stats;

            var newCharacterKinematicBody = newCharacter.GetNode<KinematicBody>("KinematicBody");
            newCharacterKinematicBody.Transform = new Transform(newCharacterKinematicBody.Transform.basis, spawnPosition);

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
			inputManager.Connect(SignalNames.Characters.MOVE_TO, newCharacterKinematicBody, SignalNames.Characters.MOVE_TO_METHOD);

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