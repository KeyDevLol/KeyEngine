using System.Globalization;
using System.Runtime.CompilerServices;
using TKVector2 = OpenTK.Mathematics.Vector2i;

namespace KeyEngine.Mathematics
{
    [Serializable]
    public struct Vector2Int : IEquatable<Vector2Int>, IFormattable
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2Int(int value)
        {
            X = value;
            Y = value;
        }

        public static Vector2Int Max(Vector2Int lhs, Vector2Int rhs) => new Vector2Int(Math.Max(lhs.X, rhs.X), Math.Max(lhs.Y, rhs.Y));
        public static Vector2Int Min(Vector2Int lhs, Vector2Int rhs) => new Vector2Int(Math.Min(lhs.X, rhs.X), Math.Min(lhs.Y, rhs.Y));
        public readonly float Magnitude => (float)Math.Sqrt(X * X + Y * Y);
        public readonly float SqrMagnitude => X * X + Y * Y;

        // +
        public static Vector2Int operator +(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2Int operator +(Vector2Int value)
        {
            return value;
        }

        // -
        public static Vector2Int operator -(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2Int operator -(Vector2Int value)
        {
            return new Vector2Int(-value.X, -value.Y);
        }

        // *
        public static Vector2Int operator *(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(left.X * right.X, left.Y * right.Y);
        }

        public static Vector2Int operator *(Vector2Int left, int scale)
        {
            return new Vector2Int(left.X * scale, left.Y * scale);
        }

        public static Vector2Int operator *(int scale, Vector2Int left)
        {
            return new Vector2Int(left.X * scale, left.Y * scale);
        }

        // /
        public static Vector2Int operator /(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(left.X / right.X, left.Y / right.Y);
        }

        public static Vector2Int operator /(Vector2Int left, int scale)
        {
            return new Vector2Int(left.X / scale, left.Y / scale);
        }

        public static Vector2Int operator /(int numerator, Vector2Int left)
        {
            return new Vector2Int(numerator / left.X, numerator / left.Y);
        }

        public static bool operator ==(Vector2Int left, Vector2Int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2Int left, Vector2Int right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Vector2(Vector2Int value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static implicit operator TKVector2(Vector2Int value)
        {
            return Unsafe.BitCast<Vector2Int, TKVector2>(value);
        }

        public static implicit operator Vector2Int(TKVector2 value)
        {
            return Unsafe.BitCast<TKVector2, Vector2Int>(value);
        }

        #region Object overrides & Interfaces

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Vector2Int value && Equals(value);
        }

        public readonly bool Equals(Vector2Int other)
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

        public static Vector2Int Zero { get => new Vector2Int(0, 0); }
        public static Vector2Int One { get => new Vector2Int(1, 1); }
        public static Vector2Int Left { get => new Vector2Int(-1, 0); }
        public static Vector2Int Right { get => new Vector2Int(1, 0); }
        public static Vector2Int Up { get => new Vector2Int(0, 1); }
        public static Vector2Int Down { get => new Vector2Int(0, -1); }
    }
}