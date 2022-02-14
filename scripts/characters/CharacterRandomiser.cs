using FaffLatest.scripts.shared;
using FaffLatest.scripts.weapons;
using Godot;
using System;

    namespace FaffLatest.scripts.characters
{
    public class CharacterRandomiser
    {
        private CharacterGeneratorStats stats;

        public CharacterRandomiser(CharacterGeneratorStats stats)
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

            GD.Print($"Generated character {newCharacter}");
            return newCharacter;
        }

    }

    public static class CharacterGeneratorStatsExtensions
    {
        public static int GetRandomHealth(this CharacterGeneratorStats stats) => RandomHelper.RNG.RandiRange(stats.MinHealth, stats.MaxHealth);

        public static string GetRandomName(this CharacterGeneratorStats stats) => stats.PossibleCharacterNames.GetRandomFromArray();

        public static AtlasTexture GetRandomFaceIcon(this CharacterGeneratorStats stats) => stats.PossibleFaceIcons.GetRandomFromArray();

        public static Weapon GetRandomWeapon(this CharacterGeneratorStats stats) => stats.PossibleStartWeapons.GetRandomFromArray();
    }
}
