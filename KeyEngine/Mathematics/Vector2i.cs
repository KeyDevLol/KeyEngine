using System.Globalization;
using System.Runtime.CompilerServices;
using TKVector2 = OpenTK.Mathematics.Vector2i;

namespace KeyEngine.Mathematics
{
    [Serializable]
    public struct Vector2i : IEquatable<Vector2i>, IFormattable
    {
        public int X;
        public int Y;

        public Vector2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2i(int value)
        {
            X = value;
            Y = value;
        }

        public static Vector2i Max(Vector2i lhs, Vector2i rhs) => new Vector2i(Math.Max(lhs.X, rhs.X), Math.Max(lhs.Y, rhs.Y));
        public static Vector2i Min(Vector2i lhs, Vector2i rhs) => new Vector2i(Math.Min(lhs.X, rhs.X), Math.Min(lhs.Y, rhs.Y));
        public readonly float Magnitude => (float)Math.Sqrt(X * X + Y * Y);
        public readonly float SqrMagnitude => X * X + Y * Y;

        // +
        public static Vector2i operator +(Vector2i left, Vector2i right)
        {
            return new Vector2i(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2i operator +(Vector2i value)
        {
            return value;
        }

        // -
        public static Vector2i operator -(Vector2i left, Vector2i right)
        {
            return new Vector2i(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2i operator -(Vector2i value)
        {
            return new Vector2i(-value.X, -value.Y);
        }

        // *
        public static Vector2i operator *(Vector2i left, Vector2i right)
        {
            return new Vector2i(left.X * right.X, left.Y * right.Y);
        }

        public static Vector2i operator *(Vector2i left, int scale)
        {
            return new Vector2i(left.X * scale, left.Y * scale);
        }

        public static Vector2i operator *(int scale, Vector2i left)
        {
            return new Vector2i(left.X * scale, left.Y * scale);
        }

        // /
        public static Vector2i operator /(Vector2i left, Vector2i right)
        {
            return new Vector2i(left.X / right.X, left.Y / right.Y);
        }

        public static Vector2i operator /(Vector2i left, int scale)
        {
            return new Vector2i(left.X / scale, left.Y / scale);
        }

        public static Vector2i operator /(int numerator, Vector2i left)
        {
            return new Vector2i(numerator / left.X, numerator / left.Y);
        }

        public static bool operator ==(Vector2i left, Vector2i right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2i left, Vector2i right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Vector2(Vector2i value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static implicit operator TKVector2(Vector2i value)
        {
            return Unsafe.BitCast<Vector2i, TKVector2>(value);
        }

        public static implicit operator Vector2i(TKVector2 value)
        {
            return Unsafe.BitCast<TKVector2, Vector2i>(value);
        }

        #region Object overrides & Interfaces

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Vector2i value && Equals(value);
        }

        public readonly bool Equals(Vector2i other)
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

        #endregion Object overrides & Interfaces

        public static Vector2i Zero { get => new Vector2i(0, 0); }
        public static Vector2i One { get => new Vector2i(1, 1); }
        public static Vector2i Left { get => new Vector2i(-1, 0); }
        public static Vector2i Right { get => new Vector2i(1, 0); }
        public static Vector2i Up { get => new Vector2i(0, 1); }
        public static Vector2i Down { get => new Vector2i(0, -1); }
    }
}