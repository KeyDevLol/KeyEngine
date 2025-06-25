
using KeyEngine.Editor.Serialization;

namespace KeyEngine
{
    public struct Color : IEquatable<Color>, ISerializable
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public Color(float r, float g, float b, float a)
        {
            R = ToByte(r);
            G = ToByte(g);
            B = ToByte(b);
            A = ToByte(a);
        }

        public Color(float r, float g, float b)
        {
            R = ToByte(r);
            G = ToByte(g);
            B = ToByte(b);
            A = 255;
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

        public static Color operator +(Color left, Color right)
        {
            return new Color((byte)(left.R + right.R), (byte)(left.G + right.G), (byte)(left.B + right.B), (byte)(left.A + right.A));
        }

        public static Color operator -(Color left, Color right)
        {
            return new Color((byte)(left.R - right.R), (byte)(left.G - right.G), (byte)(left.B - right.B), (byte)(left.A - right.A));
        }

        public static Color operator -(Color value)
        {
            return new Color(-value.R, -value.G, -value.B, -value.A);
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !left.Equals(right);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(A, R, G, B);
        }

        public readonly bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Color color && Equals(color);
        }

        public override readonly string ToString()
        {
            return string.Format("R:{0} G:{1} B:{2} A:{3}", R, G, B, A);
        }

        public static float ToFloat(byte component)
        {
            float value = (float)(component / 255f);
            return value;
        }

        public static byte ToByte(float component)
        {
            int value = (int)(component * 255f);
            //Log.Print(value);
            return (byte)Mathf.Clamp(value, 0, 255);
        }

        public void SerializeWrite(ref BinaryWriter writer)
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

        public static readonly Color red = new Color(255, 0, 0);
        public static readonly Color green = new Color(0, 255, 0);
        public static readonly Color blue = new Color(0, 0, 255);
        public static readonly Color white = new Color(255, 255, 255);
        public static readonly Color transparent = new Color(0, 0, 0, 0);
    }
}
