using System.Diagnostics.CodeAnalysis;

namespace KeyEngine.Mathematics
{
    public struct Color01 : IEquatable<Color01>, IFormattable
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color01(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color01(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
            A = 1;
        }

        public Color01(Color32 color)
        {
            R = ToFloat(color.R);
            G = ToFloat(color.G);
            B = ToFloat(color.B);
            A = ToFloat(color.A);
        }

        public float this[int index]
        {
            readonly get
            {
                return index switch
                {
                    0 => R,
                    1 => G,
                    2 => B,
                    3 => A,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Color run from 0 to 3, inclusive."),
                };
            }

            set
            {
                switch (index)
                {
                    case 0: R = value; break;
                    case 1: G = value; break;
                    case 2: B = value; break;
                    case 3: A = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Color run from 0 to 3, inclusive.");
                }
            }
        }

        public static Color01 operator +(Color01 left, Color01 right)
        {
            return new Color01(
                left.R + right.R,
                left.G + right.G, 
                left.B + right.B, 
                left.A + right.A);
        }

        public static Color01 operator -(Color01 left, Color01 right)
        {
            return new Color01(
                left.R - right.R,
                left.G - right.G,
                left.B - right.B,
                left.A - right.A);
        }

        public static Color01 operator -(Color01 value)
        {
            return new Color01(-value.R, -value.G, -value.B, -value.A);
        }

        public static bool operator ==(Color01 left, Color01 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color01 left, Color01 right)
        {
            return !(left == right);
        }

        public static implicit operator Color01(Color32 color32) => color32.AsColor01();

        public readonly bool Equals(Color01 other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Color01 color && Equals(color);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }

        public override readonly string ToString()
        {
            return string.Format("R:{0:F3} G:{1:F3} B:{2:F3} A:{3:F3}", R, G, B, A);
        }

        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            return string.Format(formatProvider, "R:{0:F3} G:{1:F3} B:{2:F3} A:{3:F3}", R, G, B, A);
        }

        private static float ToFloat(byte component)
        {
            float value = (float)(component / 255f);
            return value;
        }

        public readonly Color32 AsColor32() => new Color32(this);

        public static readonly Color01 Red = new Color01(1, 0, 0);
        public static readonly Color01 Green = new Color01(0, 1, 0);
        public static readonly Color01 Blue = new Color01(0, 0, 1);
        public static readonly Color01 White = new Color01(1, 1, 1);
        public static readonly Color01 Black = new Color01(0, 0, 0);
        public static readonly Color01 Pink = new Color01(1, 0, 1);
        public static readonly Color01 Yellow = new Color01(1, 1, 0);
        public static readonly Color01 Cyan = new Color01(0, 1, 1);
        public static readonly Color01 Transparent = new Color01(0, 0, 0, 0);
    }
}
