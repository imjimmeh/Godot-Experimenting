using FaffLatest.scripts.weapons;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaffLatest.scripts.characters
{
    public class CharacterGeneratorStats : Resource
    {
        private RandomNumberGenerator numberGenerator;
        private bool initialised = false;

        public CharacterGeneratorStats(string[] possibleCharacterNames = null, AtlasTexture[] possibleFaceIcons = null, Weapon[] possibleStartWeapons = null, int minHealth = 5, int maxHealth = 10)
        {
            Initialise();

            SetValues(possibleCharacterNames, possibleFaceIcons, possibleStartWeapons, minHealth, maxHealth);
        }

        public CharacterGeneratorStats()
        {
        }

        private void SetValues(string[] possibleCharacterNames, AtlasTexture[] possibleFaceIcons, Weapon[] possibleStartWeapons, int minHealth, int maxHealth)
        {
            PossibleCharacterNames = possibleCharacterNames;
            PossibleFaceIcons = possibleFaceIcons;
            PossibleStartWeapons = possibleStartWeapons;
            MinHealth = minHealth;
            MaxHealth = maxHealth;
        }

        private void Initialise()
        {
            numberGenerator = new RandomNumberGenerator();
            initialised = true;
        }

        [Export]
        public string[] PossibleCharacterNames { get; private set; }

        [Export]
        public AtlasTexture[] PossibleFaceIcons { get; private set; }

        [Export]
        public Weapon[] PossibleStartWeapons { get; private set; }

        [Export]
        public int MinHealth { get; private set; } = 5;

        [Export]
        public int MaxHealth { get; private set; } = 15;
    }
}
