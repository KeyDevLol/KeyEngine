using OpenTK.Graphics.OpenGL;

namespace KeyEngine
{
    public struct Glyph : IDisposable
    {
        public readonly Vector2 Size;
        public readonly Vector2 Bearing;

        public readonly int TextureHandle;
        public readonly nint Advance;

        private bool disposed;

        public Glyph(int textureHandle, Vector2 size, Vector2 bearing, nint advance)
        {
            Size = size;
            Bearing = bearing;
            Advance = advance;
            TextureHandle = textureHandle;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            GL.DeleteTexture(TextureHandle);
            GC.SuppressFinalize(this);

            disposed = true;
        }
    }
}
