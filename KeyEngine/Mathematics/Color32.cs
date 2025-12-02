using System.Diagnostics.CodeAnalysis;

namespace KeyEngine.Mathematics
{
    public struct Color32 : IEquatable<Color32>
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color32(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color32(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public Color32(float r, float g, float b, float a)
        {
            R = ToByte(r);
            G = ToByte(g);
            B = ToByte(b);
            A = ToByte(a);
        }

        public Color32(float r, float g, float b)
        {
            R = ToByte(r);
            G = ToByte(g);
            B = ToByte(b);
            A = 255;
        }

        public Color32(Color01 color)
        {
            R = ToByte(color.R);
            G = ToByte(color.G);
            B = ToByte(color.B);
            A = ToByte(color.A);
        }

        public byte this[int index]
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

        public static Color32 operator +(Color32 left, Color32 right)
        {
            return new Color32((byte)(left.R + right.R), (byte)(left.G + right.G), (byte)(left.B + right.B), (byte)(left.A + right.A));
        }

        public static Color32 operator -(Color32 left, Color32 right)
        {
            return new Color32((byte)(left.R - right.R), (byte)(left.G - right.G), (byte)(left.B - right.B), (byte)(left.A - right.A));
        }

        public static Color32 operator -(Color32 value)
        {
            return new Color32(-value.R, -value.G, -value.B, -value.A);
        }

        public static bool operator ==(Color32 left, Color32 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color32 left, Color32 right)
        {
            return !(left == right);
        }

        public static implicit operator Color32(Color01 color01) => color01.AsColor32();

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }

        public readonly bool Equals(Color32 other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is Color32 color && Equals(color);

        public override readonly string ToString()
        {
            return string.Format("R:{0} G:{1} B:{2} A:{3}", R, G, B, A);
        }

        private static byte ToByte(float component)
        {
            int value = (int)(component * 255f);
            return (byte)Mathf.Clamp(value, 0, 255);
        }

        public readonly Color01 AsColor01() => new Color01(this);

        public readonly void SerializeWrite(ref BinaryWriter writer)
        {
            writer.Write(R);
            writer.Write(G);
            writer.Write(B);
            writer.Write(A);
        }

        public void SerializeRead(ref BinaryReader reader)
        {
            R = reader.ReadByte();
            G = reader.ReadByte();
            B = reader.ReadByte();
            A = reader.ReadByte();
        }

        public static readonly Color32 Red = new Color32(255, 0, 0);
        public static readonly Color32 Green = new Color32(0, 255, 0);
        public static readonly Color32 Blue = new Color32(0, 0, 255);
        public static readonly Color32 White = new Color32(255, 255, 255);
        public static readonly Color32 Black = new Color32(0, 0, 0);
        public static readonly Color32 Pink = new Color32(255, 0, 255);
        public static readonly Color32 Yellow = new Color32(255, 255, 0);
        public static readonly Color32 Cyan = new Color32(0, 255, 255);
        public static readonly Color32 Transparent = new Color32(0, 0, 0, 0);
    }
}
