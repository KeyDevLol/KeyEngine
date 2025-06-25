using OpenTK.Graphics.OpenGL;

namespace KeyEngine.Rendering
{
    /// <summary>
    /// VBO Abstraction
    /// </summary>
    public class VertexBufferObject : IBuffer, IDisposable
    {
        public int Handle { get; }
        private bool disposed;

        public VertexBufferObject()
        {
            Handle = GL.GenBuffer();
        }

#if WARN_MEMORY_LEAKS
        ~VertexBufferObject()
        {
            Log.Assert(disposed, $"{nameof(VertexBufferObject)} was finalized but not disposed, this is a memory leak.", LogType.Warning);
        }
#endif // WARN_MEMORY_LEAKS

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            GL.DeleteBuffer(Handle);
            GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}
