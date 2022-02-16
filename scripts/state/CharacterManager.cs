using System;
using System.Collections.Generic;
using FaffLatest.scripts.characters;
using Godot;

namespace FaffLatest.scripts.state{
    public class CharacterManager : Node
    {
        public static CharacterManager Instance;

        private HashSet<Character> aiCharacters;
        private HashSet<Character> playerCharacters;

        public HashSet<Character> AiCharacters => aiCharacters;
        public HashSet<Character> PlayerCharacters => playerCharacters;

        [Export]
        public int MaxAiCharacters { get; private set; } = 100;

        public override void _Ready()
        {
            base._Ready();

            Instance = this;
        }

        public void InitialiseCollections(int playerCharacterCount)
        {
            aiCharacters = new HashSet<Character>(MaxAiCharacters);
            playerCharacters = new HashSet<Character>(playerCharacterCount);
        }

        public void AddCharacter(Character character)
            => MethodToAddCharacter(character);

        public Action<Character> MethodToAddCharacter(Character character) => character.Stats.IsPlayerCharacter switch
        {
            true => AddPlayerCharacter,
            false => AddAiCharacter
        };
    
        public void AddAiCharacter(Character character)
            => aiCharacters.Add(character);

        public void AddPlayerCharacter(Character character)
            => playerCharacters.Add(character);
    }
}