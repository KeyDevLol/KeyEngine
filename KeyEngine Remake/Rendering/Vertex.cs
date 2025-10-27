using KeyEngine.Mathematics;
using System.Diagnostics.CodeAnalysis;

namespace KeyEngine.Rendering
{
    public struct Vertex : IEquatable<Vertex>, IFormattable
    {
        public Vector2 Position;
        public Color01 Color;
        public Vector2 UV;

        public Vertex(Vector2 position, Color01 color, Vector2 uv)
        {
            Position = position;
            Color = color;
            UV = uv;
        }

        public Vertex(float x, float y, float r, float g, float b, float a, float u, float v)
        {
            Position = new Vector2(x, y);
            Color = new Color01(r, g, b, a);
            UV = new Vector2(u, v);
        }

        public readonly float[] AsArray()
        {
            return [Position.X, Position.Y, Color.R, Color.G, Color.B, Color.A, UV.X, UV.Y];
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Vertex vertex && Equals(vertex);
        }

        public readonly bool Equals(Vertex other)
        {
            return Position == other.Position && Color == other.Color && UV == other.UV;
        }

        public static bool operator ==(Vertex left, Vertex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vertex left, Vertex right)
        {
            return !(left == right);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Position, Color, UV);
        }

        public readonly override string ToString()
        {
            return string.Format("Position: {0} Color: {1} UV: {2}", Position, Color, UV);
        }

        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            return string.Format(formatProvider, "Position: {0} Color: {1} UV: {2}", Position, Color, UV);
        }
    }
}
