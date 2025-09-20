using System.Diagnostics.CodeAnalysis;

namespace KeyEngine.Rendering
{
    public struct Vertex : IEquatable<Vertex>
    {
        public float X;
        public float Y;

        public float R;
        public float G;
        public float B;
        public float A;

        public float U;
        public float V;

        public Vertex(float x, float y, float r, float g, float b, float a, float u, float v)
        {
            X = x;
            Y = y;

            R = r;
            G = g;
            B = b;
            A = a;

            U = u;
            V = v;
        }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Vertex vertex && Equals(vertex);
        }

        public readonly bool Equals(Vertex other)
        {
            return X == other.X && Y == other.Y && R == other.R && G == other.G && B == other.B && A == other.A && U == other.U && V == other.V;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y, R, G, B, A, U, V);
        }
        public static bool operator ==(Vertex left, Vertex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vertex left, Vertex right)
        {
            return !(left == right);
        }
    }
}
