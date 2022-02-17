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

        public CharacterManager InitialiseCollections(int playerCharacterCount)
        {
            aiCharacters = new HashSet<Character>(MaxAiCharacters);
            playerCharacters = new HashSet<Character>(playerCharacterCount);

            
            return this;
        }

        public bool AddCharacter(Character character) => character.Stats.IsPlayerCharacter switch
        {
            true => AddPlayerCharacter(character),
            false => AddAiCharacter(character)
        };
    
        public bool AddAiCharacter(Character character) => aiCharacters.Add(character);

        public bool AddPlayerCharacter(Character character) => playerCharacters.Add(character);
        
        
        private void _On_Character_Disposing(Character character) => RemoveCharacter(character);

        public bool RemoveCharacter(Character character) => character.Stats.IsPlayerCharacter switch
        {
            true => RemovePlayerCharacter(character),
            false => RemoveAiCharacter(character)
        };
    
        public bool RemoveAiCharacter(Character character) => aiCharacters.Remove(character);

        public bool RemovePlayerCharacter(Character character) => playerCharacters.Remove(character);
        
    }
}