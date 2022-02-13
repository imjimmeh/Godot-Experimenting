using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaffLatest.scripts.characters;
using FaffLatest.scripts.state;
using Godot;

namespace FaffLatest.scripts.input
{
    public static class InputHelpers
    {
        public static bool IsAttackCommand(this InputEventMouseButton mouseButtonEvent, GameStateManager gameStateManager, Character targetCharacter)
            => IsLMB(mouseButtonEvent) && gameStateManager.HaveACharacterSelected && gameStateManager.SelectedCharacterCanUseAction && !targetCharacter.Stats.IsPlayerCharacter && gameStateManager.CurrentTurn == Faction.PLAYER;

        public static bool IsLMB(this InputEventMouseButton mouseButtonEvent) => mouseButtonEvent.ButtonIndex == 1;

        public static bool IsSelectCharacterAction(this InputEventMouseButton mouseButtonEvent, GameStateManager gameStateManager, Character clickedCharacter)
            => mouseButtonEvent.IsLMB() && clickedCharacter.Stats.IsPlayerCharacter && !gameStateManager.CharacterIsActive && gameStateManager.CurrentTurn == Faction.PLAYER;
    }
}
