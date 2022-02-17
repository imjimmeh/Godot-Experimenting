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
		public delegate void _Characters_Spawned();

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
		private Node playerCharactersRoot;
		private RandomNumberGenerator random;
		private UIElementContainer ui;

		public override void _Ready()
		{
			FindNeededNodes();

			random = new RandomNumberGenerator();
			random.Randomize();


			base._Ready();
		}

		public async Task SpawnCharacters(CharacterStats[] charactersToCreate, SpawnableAreas spawnArea)
		{
			for (var x = 0; x < charactersToCreate.Length; x++)
			{
				if (charactersToCreate[x] == null)
					break;

				var spawnPosition = spawnArea.GetSpawnPosition(charactersToCreate[x]);

				var character = await SpawnCharacter(charactersToCreate[x], spawnPosition);

                if (!character.Stats.IsPlayerCharacter)
                    AiSetup(character);

            }

			EmitSignal(SignalNames.Loading.CHARACTERS_SPAWNED);
		}

        private void AiSetup(Character character)
        {
            character.Stats.EquippedWeapon = ZombieWeapon;
			
			var aicontroller = new AiCharacterController();
			character.ProperBody.CallDeferred("add_child", aicontroller);
			aicontroller.Name = "AiCharacterController";
			aicontroller.Connect(nameof(AiCharacterController._AiCharacter_TurnFinished), AIManager.Instance, "_On_AiCharacter_TurnFinished");
        }

        public async Task<Character> SpawnCharacter(CharacterStats stats, Vector3 spawnPosition)
        {
            Character character = InitialiseCharacterFromStats(stats);

            await On_SpawnedCharacter_Ready(spawnPosition, character);

            return character;
        }

        private Character InitialiseCharacterFromStats(CharacterStats stats)
        {
            var character = CharacterBase.Instance<Character>();
            character.Stats = stats;

            var root = stats.IsPlayerCharacter ? playerCharactersRoot : aiCharactersRoot;
            var groupName = stats.IsPlayerCharacter ? GroupNames.PLAYER_CHARACTERS : GroupNames.AI_CHARACTERS;

            root.CallDeferred("add_child", character);
            character.CallDeferred("add_to_group", groupName);
            return character;
        }

        private async Task On_SpawnedCharacter_Ready(Vector3 spawnPosition, Character character)
        {
            await ToSignal(character, "ready");

            var body = character.ProperBody;
            SetPosition(spawnPosition, body);
            AddCharacterSignals(body, character);
            astarNavigator._On_Character_Created(character);

            CharacterManager.Instance.AddCharacter(character);
        }

        private static void SetPosition(Vector3 spawnPosition, Spatial character)
			=> character.Transform = new Transform(character.Transform.basis, spawnPosition);
		
        private void AddCharacterSignals(Node newCharacterKinematicBody, Character character)
		{
			var pathMover = newCharacterKinematicBody.GetNode("PathMover");

			inputManager.Connect(SignalNames.Characters.MOVE_TO, pathMover,  SignalNames.Characters.MOVE_TO_METHOD);

			character.Connect(SignalNames.Characters.RECEIVED_DAMAGE, ui, SignalNames.Characters.RECEIVED_DAMAGE_METHOD);
			character.Connect("_Character_Disposing", GameStateManager.Instance, "_On_Character_Disposing");
			character.Connect("_Character_Disposing", AStarNavigator.Instance, "_On_Character_Disposing");
			character.Connect("_Character_Disposing", CharacterManager.Instance, "_On_Character_Disposing");

			if(!character.Stats.IsPlayerCharacter)
				character.Connect("_Character_Disposing", AIManager.Instance, "_On_Character_Disposing");


			newCharacterKinematicBody.Connect(SignalNames.Characters.CLICKED_ON, inputManager, SignalNames.Characters.CLICKED_ON_METHOD);
			newCharacterKinematicBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, astarNavigator, SignalNames.Characters.MOVEMENT_FINISHED_METHOD);
			newCharacterKinematicBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, characterMovementPathManager, SignalNames.Characters.MOVEMENT_FINISHED_METHOD);
			newCharacterKinematicBody.Connect(SignalNames.Characters.MOVEMENT_FINISHED, GameStateManager.Instance, SignalNames.Characters.MOVEMENT_FINISHED_METHOD);

			var bodyMesh = newCharacterKinematicBody.GetNode("CSGBox");

			GameStateManager.Instance.Connect(SignalNames.Characters.SELECTED, bodyMesh, SignalNames.Characters.SELECTED_METHOD);
			GameStateManager.Instance.Connect(SignalNames.Characters.SELECTION_CLEARED, bodyMesh, SignalNames.Characters.SELECTION_CLEARED_METHOD);


		}

		private void FindNeededNodes()
		{
			aiCharactersRoot = GetNode(NodeReferences.BaseLevel.AI_ROOT);
			astarNavigator = AStarNavigator.Instance;
			characterMovementPathManager = GetNode<CharacterMovementPathManager>(NodeReferences.BaseLevel.Effects.MOVEMENT_PATH);
			inputManager = GetNode(NodeReferences.Systems.INPUT_MANAGER);
			playerCharactersRoot = GetNode(NodeReferences.BaseLevel.PLAYER_ROOT);
			ui = GetNode<UIElementContainer>(NodeReferences.BaseLevel.UI);
		}
    }
}
