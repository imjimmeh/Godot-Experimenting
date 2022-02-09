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

        public static CharacterStats GenerateRandomCharacter()
        {
            if (!initialised)
            {
                numberGenerator.Randomize();
                faceIcon = ResourceLoader.Load(faceIconResPath, "AtlasTexture") as AtlasTexture;
                initialised = true;
            }

            var stats = new CharacterStats();
            stats.SetName(GetRandomName(numberGenerator.RandiRange(0, names.Length - 1)));
            stats.SetMaxMovementDistance(numberGenerator.RandiRange(4, 8));
            stats.SetPlayerCharacter(true);
            stats.FaceIcon = faceIcon;
        
            return stats;
        }

        private static string GetRandomName(int toGet) => names[toGet];
    }
}
