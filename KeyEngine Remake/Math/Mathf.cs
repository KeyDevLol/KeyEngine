namespace KeyEngine
{
    public struct Mathf
    {
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }

        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = Math.Max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
            float change = current - target;
            float originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            change = Mathf.Clamp(change, -maxChange, maxChange);
            target = current - change;

            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            float output = target + (change + temp) * exp;

            // Prevent overshooting
            if (originalTo - current > 0.0F == output > originalTo)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;

            else if (value > max)
                value = max;
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;

            else if (value > max)
                value = max;
            return value;
        }

        public static float Clamp01(float value)
        {
            if (value < 0f)
                return 0f;
            else if (value > 1f)
                return 1f;
            else
                return value;
        }

        public static float Repeat(float t, float length)
        {
            return Clamp(t - Floor(t / length) * length, 0.0f, length);
        }

        public static float Pow(float f, float p) { return (float)Math.Pow(f, p); }

        public static float Exp(float power) { return (float)Math.Exp(power); }

        public static float Log(float f, float p) { return (float)Math.Log(f, p); }

        public static float Log(float f) { return (float)Math.Log(f); }

        public static float Log10(float f) { return (float)Math.Log10(f); }

        public static float Ceil(float f) { return (float)Math.Ceiling(f); }

        public static float Floor(float f) { return (float)Math.Floor(f); }

        public static float Round(float f) { return (float)Math.Round(f); }

        public static int CeilToInt(float f) { return (int)Math.Ceiling(f); }

        public static int FloorToInt(float f) { return (int)Math.Floor(f); }

        public static int RoundToInt(float f) { return (int)Math.Round(f); }

        public static float Sign(float f) { return f >= 0F ? 1F : -1F; }

        public const float PI = (float)Math.PI;

        public const float INFINITY = float.PositiveInfinity;

        public const float NEGITIVE_INFINITY = float.NegativeInfinity;

        public const float DEG_2_RAD = PI * 2F / 360F;

        public const float RED_2_DEG = 1F / DEG_2_RAD;

        internal const int kMaxDecimals = 15;
    }
}
