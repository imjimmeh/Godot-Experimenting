﻿using System;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.constants;
using FaffLatest.scripts.movement;
using Godot;

namespace FaffLatest.scripts.state
{
    public class GameStateManager : Node
    {
        private Character currentlySelectedCharacter;
        private CurrentTurn currentTurn;

        public Character CurrentlySelectedCharacter => currentlySelectedCharacter;
        public CurrentTurn CurrentTurn => currentTurn;

        [Signal]
        public delegate void _Character_Selected(Character character);

        [Signal]
        public delegate void _Character_SelectionCleared();

        public AStarNavigator AStarNavigator { get; private set; }
        public SpawnManager SpawnManager { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            AStarNavigator = GetNode<AStarNavigator>("../AStarNavigator");
            SpawnManager = GetNode<SpawnManager>("../SpawnManager");

            InitialiseMap();
        }

        public void InitialiseMap()
        {
        }

        public void SetCurrentlySelectedCharacter(Character character)
        {
            currentlySelectedCharacter = character;
            EmitSignal(SignalNames.Characters.SELECTED, character);

            GD.Print($"{character.Stats.CharacterName} has been selected");
        }

        public void ClearCurrentlySelectedCharacter()
        {
            currentlySelectedCharacter = null;
            EmitSignal(SignalNames.Characters.SELECTION_CLEARED);

            GD.Print("Character has been unselected");
        }
    }
}
