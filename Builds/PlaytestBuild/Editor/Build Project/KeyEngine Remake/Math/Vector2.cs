using System.Globalization;
using System.Runtime.CompilerServices;
using TKVector2 = OpenTK.Mathematics.Vector2;
using NUMVector2 = System.Numerics.Vector2;
using KeyEngine.Editor.Serialization;

namespace KeyEngine
{
    public struct Vector2 : IEquatable<Vector2>, IFormattable, ISerializable
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        // +
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2 operator +(Vector2 value)
        {
            return value;
        }

        // -
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2 operator -(Vector2 value)
        {
            return new Vector2(-value.X, -value.Y);
        }

        // *
        public static Vector2 operator *(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X * right.X, left.Y * right.Y);
        }

        public static Vector2 operator *(Vector2 left, float scale)
        {
            return new Vector2(left.X * scale, left.Y * scale);
        }

        public static Vector2 operator *(float scale, Vector2 left)
        {
            return new Vector2(left.X * scale, left.Y * scale);
        }

        // /
        public static Vector2 operator /(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X / right.X, left.Y / right.Y);
        }

        public static Vector2 operator /(Vector2 left, float scale)
        {
            return new Vector2(left.X / scale, left.Y / scale);
        }

        public static Vector2 operator /(float numerator, Vector2 left)
        {
            return new Vector2(numerator / left.X, numerator / left.Y);
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        { 
            return left.Equals(right); 
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        { 
            return !left.Equals(right);
        }

        public static implicit operator TKVector2(Vector2 value)
        {
            return Unsafe.BitCast<Vector2, TKVector2>(value);
        }

        public static implicit operator Vector2(TKVector2 value)
        {
            return Unsafe.BitCast<TKVector2, Vector2>(value);
        }

        public static implicit operator Vector2(NUMVector2 value)
        {
            return Unsafe.BitCast<NUMVector2, Vector2>(value);
        }

        public static implicit operator NUMVector2(Vector2 value)
        {
            return Unsafe.BitCast<Vector2, NUMVector2>(value);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Vector2 value && Equals(value);
        }

        public readonly bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override readonly string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", X, Y);
        }

        public readonly string ToString(string? format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", X.ToString(format, CultureInfo.CurrentCulture), Y.ToString(format, CultureInfo.CurrentCulture));
        }

        public readonly string ToString(IFormatProvider? formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1}", X, Y);
        }

        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (format == null)
                ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1}", X.ToString(format, formatProvider), Y.ToString(format, formatProvider));
        }

        public readonly void BinaryWrite(ref BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
        }

        public void BinaryRead(ref BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();

            Log.Print($"LolOl {this.ToString()}");
        }

        public void SerializeWrite(ref BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
        }

        public void SerializeRead(ref BinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
        }

        public static Vector2 Zero { get => new Vector2(0, 0); }
        public static Vector2 One { get => new Vector2(1, 1); }
        public static Vector2 Left { get => new Vector2(-1, 0); }
        public static Vector2 Right { get => new Vector2(1, 0); }
        public static Vector2 Up { get => new Vector2(0, 1); }
        public static Vector2 Down { get => new Vector2(0, -1); }
    }
}