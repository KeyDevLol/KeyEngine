namespace KeyEngine.Mathematics
{
    public static class Rand
    {
        private static Random rand = new Random();

        public static double GenValue(double min, double max)
        {
            if (min > max || max < min)
                throw new ArgumentOutOfRangeException();

            return rand.NextDouble() * (max - min) + min;
        }

        public static float GenValue(float min, float max)
        {
            if (min > max || max < min)
                throw new ArgumentOutOfRangeException();

            return rand.NextSingle() * (max - min) + min;
        }

        public static int GenValue(int min, int max)
        {
            if (min > max || max < min)
                throw new ArgumentOutOfRangeException();

            return rand.Next(min, max);
        }
    }
}
