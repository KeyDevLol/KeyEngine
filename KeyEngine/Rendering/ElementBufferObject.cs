using OpenTK.Graphics.OpenGL;

namespace KeyEngine.Rendering
{
    /// <summary>
    /// EBO Abstraction
    /// </summary>
    public class ElementBufferObject : IBuffer, IDisposable
    {
        public int Handle { get; }
        private bool disposed;

        public ElementBufferObject()
        {
            Handle = GL.GenBuffer();
        }

#if WARN_MEMORY_LEAKS
        ~ElementBufferObject()
        {
            Log.Print("EBO Destuctor");
            Log.Assert(disposed, $"{nameof(ElementBufferObject)} was finalized but not disposed, this is a memory leak.", LogType.Warning);
        }
#endif // WARN_MEMORY_LEAKS

        public void Bind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
        }

        public void Unbind()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
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
