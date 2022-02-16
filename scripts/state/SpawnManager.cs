using FaffLatest.scripts.ai;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.effects;
using FaffLatest.scripts.map;
using FaffLatest.scripts.movement;
using FaffLatest.scripts.shared;
using FaffLatest.scripts.ui;
using FaffLatest.scripts.weapons;
using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaffLatest.scripts.state
{
	public class SpawnManager : Node
	{
		[Signal]
		public delegate void _Characters_Spawned(Node spawnManager);

		[Export]
		public Weapon ZombieWeapon { get; private set; }

		[Export]
		public Weapon[] PossibleInitialWeapons { get; private set; }

		[Export]
		public PackedScene CharacterBase { get; private set; }

		private Node aiCharactersRoot;
		private AStarNavigator astarNavigator;
		private Node characterMovementPathManager;
		private Node inputManager;
		private Node gameStateManager;
		private Node playerCharactersRoot;
		private UIElementContainer ui;

		private Character[] characters;
		public Character[] Characters { get => characters; set => characters = value; }

		private RandomNumberGenerator random;

		public override void _Ready()
		{
			FindNeededNodes();
			Preload();

			random = new RandomNumberGenerator();
			random.Randomize();


			base._Ready();
		}

		public async void SpawnCharacters(CharacterStats[] charactersToCreate, SpawnableAreas spawnArea)
		{
			var playerCharacters = new Character[charactersToCreate.Length];
			var aiChars = new Character[charactersToCreate.Length];

			int pc = 0;
			int nonPc = 0;


			for (var x = 0; x < charactersToCreate.Length; x++)
			{
				if (charactersToCreate[x] == null)
					break;

				var areaToSpawnFrom = charactersToCreate[x].IsPlayerCharacter ? spawnArea.PlayerSpawnableAreas : spawnArea.EnemySpawnableAreas;
				var spawnPosition = GetSpawnPosition(areaToSpawnFrom, random);

				spawnPosition = spawnPosition.WithValues(y: 0.5f);
				var character = await SpawnCharacter(charactersToCreate[x], spawnPosition);

				if(character.Stats.IsPlayerCharacter)
				{
					playerCharacters[pc] = character;
					pc++;
				}
				else
				{
					character.Stats.EquippedWeapon = ZombieWeapon;
					aiChars[nonPc] = character;
					nonPc++;
				}
			}

			Array.Resize(ref playerCharacters, pc);
			Array.Resize(ref aiChars, nonPc);

			this.characters = playerCharacters;

			var nodes = playerCharacters as Node[];

			GD.Print($"Setting AI Chars");
			GetNode<AIManager>("../AIManager").SetCharacters(aiChars);
			EmitSignal(SignalNames.Loading.CHARACTERS_SPAWNED, this);
		}

		public async Task<Character> SpawnCharacter(CharacterStats stats, Vector3 spawnPosition)
        {
            var character = CharacterBase.Instance<Character>();
            character.Stats = stats;

            var body = SetPosition(spawnPosition, character);
            var root = stats.IsPlayerCharacter ? playerCharactersRoot : aiCharactersRoot;
            var groupName = stats.IsPlayerCharacter ? GroupNames.PLAYER_CHARACTERS : GroupNames.AI_CHARACTERS;

            root.CallDeferred("add_child", character);
            character.CallDeferred("add_to_group", groupName);

            AddCharacterSignals(body, character);

            await ToSignal(character, "ready");
            astarNavigator._On_Character_Created(character);

            return character;
        }

        private static KinematicBody SetPosition(Vector3 spawnPosition, Character character)
        {
            var newCharacterKinematicBody = character.GetNode<KinematicBody>("KinematicBody");
            newCharacterKinematicBody.Transform = new Transform(newCharacterKinematicBody.Transform.basis, spawnPosition);

            return newCharacterKinematicBody;
        }

        private void AddCharacterSignals(Node newCharacterKinematicBody, Character character)
		{
			var pathMover = newCharacterKinematicBody.GetNode("PathMover");

			inputManager.Connect(SignalNames.Characters.MOVE_TO, pathMover,  SignalNames.Characters.MOVE_TO_METHOD);

			character.Connect(SignalNames.Characters.RECEIVED_DAMAGE, ui, SignalNames.Characters.RECEIVED_DAMAGE_METHOD);
			character.Connect("_Character_Disposing", gameStateManager, "_On_Character_Disposing");

			newCharacterKinematicBody.Connect(SignalNames.Characters.CLICKED_ON, inputManager, SignalNames.Characters.CLICKED_ON_METHOD);
			newCharacterKinematicBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, astarNavigator, SignalNames.Characters.MOVEMENT_FINISHED_METHOD);
			newCharacterKinematicBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, characterMovementPathManager, SignalNames.Characters.MOVEMENT_FINISHED_METHOD);
			newCharacterKinematicBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, gameStateManager, SignalNames.Characters.MOVEMENT_FINISHED_METHOD);

			var bodyMesh = newCharacterKinematicBody.GetNode("CSGBox");

			gameStateManager.Connect(SignalNames.Characters.SELECTED, bodyMesh, SignalNames.Characters.SELECTED_METHOD);
			gameStateManager.Connect(SignalNames.Characters.SELECTION_CLEARED, bodyMesh, SignalNames.Characters.SELECTION_CLEARED_METHOD);

		}

		private void FindNeededNodes()
		{
			aiCharactersRoot = GetNode(NodeReferences.BaseLevel.AI_ROOT);
			astarNavigator = GetNode<AStarNavigator>(NodeReferences.Systems.ASTAR);
			characterMovementPathManager = GetNode<CharacterMovementPathManager>(NodeReferences.BaseLevel.Effects.MOVEMENT_PATH);
			gameStateManager = GetNode(NodeReferences.Systems.GAMESTATE_MANAGER);
			inputManager = GetNode(NodeReferences.Systems.INPUT_MANAGER);
			playerCharactersRoot = GetNode(NodeReferences.BaseLevel.PLAYER_ROOT);
			ui = GetNode<UIElementContainer>(NodeReferences.BaseLevel.UI);
		}

		private void Preload()
		{
		}

		private static Vector3 GetSpawnPosition(List<Vector3> positions, RandomNumberGenerator random)
		{
			var posToGet = random.RandiRange(0, positions.Count - 1);

			var position = positions[posToGet];

			if(position != null)
			{
				positions.Remove(position);

				return position;
			}

			return GetSpawnPosition(positions, random);
		}
	}
}
