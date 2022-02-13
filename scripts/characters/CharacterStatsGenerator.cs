using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace FaffLatest.scripts.characters
{
    public class CharacterStatsGenerator : Godot.Reference
    {
        private const string faceIconResPath = "res://art/sprites/characterfaces/1.tres";
        private static RandomNumberGenerator numberGenerator = new RandomNumberGenerator();
        private static bool initialised = false;

        private static string[] names = { "Jim", "Kayla", "Clem", "Pippa", "Mimi", "Chloe", "Nadine", "Drake", "Sully" };

        private static AtlasTexture faceIcon;

        private const int minHealth = 5;

        private const int maxHealth = 15;

        public static CharacterStats GenerateRandomCharacter()
        {
            if (!initialised)
            {
                numberGenerator.Randomize();
                faceIcon = ResourceLoader.Load(faceIconResPath, "AtlasTexture") as AtlasTexture;
                initialised = true;
            }

            var stats = new CharacterStats();
            stats.Initialise(GetRandomName(GetRandomNumberNumber()), GetRandomHealth(), true, faceIcon, null);

            return stats;
        }

        private static int GetRandomHealth()
        {
            return numberGenerator.RandiRange(minHealth, maxHealth);
        }

        private static int GetRandomNumberNumber()
        {
            return numberGenerator.RandiRange(0, names.Length - 1);
        }

        private static string GetRandomName(int toGet) => names[toGet];
    }
}
