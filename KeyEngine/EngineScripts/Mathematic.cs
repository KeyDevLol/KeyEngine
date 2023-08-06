using SFML.System;
using SFML.Graphics;
using System;

namespace KeyEngine
{
    public partial struct KeyMath
    {
        public static float Round(float value)
        {
            return (float)Math.Round(value);
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + ((b - a) * t);
        }

        public static float Clamp01(float value)
        {
            if (value < 0)
            {
                return 0f;
            }
            else if (value > 1)
            {
                return 1f;
            }
            else
            {
                return value;
            }
        }

        public static Color ColorLerp(Color a, Color b, float t)
        {
            return new Color(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t),
                (byte)(a.A + (b.A - a.A) * t)
            );
        }

        public static Vector2f Lerp(Vector2f a, Vector2f b, float t)
        {
            return new Vector2f(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }
        public static Vector2f NormalizeVector(Vector2f vector)
        {
            double lol = 1.0f / Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            return new Vector2f(vector.X * (float)lol, vector.Y * (float)lol);
        }

        public static Vector2f MoveTowards(Vector2f current, Vector2f target, float maxDistanceDelta)
        {
            float toVector_x = target.X - current.X;
            float toVector_y = target.Y - current.Y;

            float sqDist = toVector_x * toVector_x + toVector_y * toVector_y;

            if (sqDist == 0 || (maxDistanceDelta >= 0 && sqDist <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float dist = (float)Math.Sqrt(sqDist);

            return new Vector2f(current.X + toVector_x / dist * maxDistanceDelta,
                current.Y + toVector_y / dist * maxDistanceDelta);
        }

        public static float DistanceVectors(Vector2f a, Vector2f b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}
