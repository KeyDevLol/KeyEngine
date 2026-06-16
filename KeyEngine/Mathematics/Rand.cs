using System.Numerics;

namespace KeyEngine.Mathematics
{
    public static class Rand
    {
        private static Random Rng { get; set; } = new Random();

        public static void SetSeed(int seed) => Rng = new(seed);

        public static int Range(int min, int max) => Rng.Next(min, max);
        public static double Range(double min, double max) => Lerp(min, max, Rng.NextDouble());
        public static float Range(float min, float max) => Lerp(min, max, Rng.NextSingle());

        private static T Lerp<T>(T min, T max, T range01) where T : INumber<T>
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max, nameof(min));
            return min + (max - min) * range01;
        }
    }
}
