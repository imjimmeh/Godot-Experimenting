using Godot;
using System;

namespace FaffLatest.scripts.shared
{
    public static class RandomHelper
    {
        private static bool isInitialised = false;
        private static RandomNumberGenerator rng;

        public static RandomNumberGenerator RNG
        {
            get
            {
                if (rng == null || !isInitialised)
                    InitialiseRNG();

                return rng;
            }
        }

        private static void InitialiseRNG()
        {
            rng = new RandomNumberGenerator();
            rng.Randomize();
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

            return RNG.RandiRange(0, array.Length - 1);
        }
    }
}
