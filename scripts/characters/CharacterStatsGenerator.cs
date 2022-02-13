using FaffLatest.scripts.weapons;
using Godot;
using System;

namespace FaffLatest.scripts.characters
{
    public class CharacterStatsGenerator
    {
        private CharacterGeneratorStats stats;

        public CharacterStatsGenerator(CharacterGeneratorStats stats)
        {
            this.stats = stats ?? throw new ArgumentNullException(nameof(stats));
        }

        public CharacterStats GenerateRandomCharacter()
        {
            var newCharacter = new CharacterStats();

            newCharacter.Initialise(
                name: stats.GetRandomName(),
                maxHealth: stats.GetRandomHealth(),
                isPlayerCharacter: true,
                faceIcon: stats.GetRandomFaceIcon(),
                weapon: stats.GetRandomWeapon());

            return newCharacter;
        }

    }

    public static class CharacterGeneratorStatsExtensions
    {
        public static int GetRandomHealth(this CharacterGeneratorStats stats) => Random.RandomNumberGenerator.RandiRange(stats.MinHealth, stats.MaxHealth);

        public static string GetRandomName(this CharacterGeneratorStats stats) => stats.PossibleCharacterNames.GetRandomFromArray();

        public static AtlasTexture GetRandomFaceIcon(this CharacterGeneratorStats stats) => stats.PossibleFaceIcons.GetRandomFromArray();

        public static Weapon GetRandomWeapon(this CharacterGeneratorStats stats) => stats.PossibleStartWeapons.GetRandomFromArray();
    }

    public static class Random
    {
        private static bool isInitialised = false;
        private static RandomNumberGenerator rng;

        public static RandomNumberGenerator RandomNumberGenerator { get => GetRNG(); }
        
        private static RandomNumberGenerator GetRNG()
        {
            if (rng == null)
                InitialiseRNG();

            return rng;
        }

        private static void InitialiseRNG()
        {
            rng = new RandomNumberGenerator();
            RandomNumberGenerator.Randomize();
            isInitialised = true;
        }

        public static T GetRandomFromArray<T>(this T[] array)
        {
            int indexToGet = array.GetRandomIndexForArray();
            return array[indexToGet];
        }

        public static int GetRandomIndexForArray<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
                throw new Exception("Array is null or empty");

            return RandomNumberGenerator.RandiRange(0, array.Length - 1);
        }
    }
}
