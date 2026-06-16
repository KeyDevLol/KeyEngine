using NAudio.Codecs;
using System.Numerics;

namespace KeyEngine.Mathematics
{
    public static class Rand
    {
        private static Random rand = new Random();
        
        public static int Range(int min, int max) => rand.Next(min, max);
        public static double Range(double min, double max) => Lerp(min, max, rand.NextSingle());
        public static float Range(float min, float max) => Lerp(min, max, rand.NextSingle());

        private static T Lerp<T>(T min, T max, T range01) where T : INumber<T>
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max, nameof(min));
            return min + (max - min) * range01;
        }
    }
}
