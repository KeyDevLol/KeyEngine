using OpenTK.Graphics.OpenGL;
using System.Xml.Linq;

namespace KeyEngine.Rendering
{
    /// <summary>
    /// VAO Abstraction
    /// </summary>
    public class VertexAttributeObject : IBuffer, IDisposable
    {
        public int Handle { get; }
        private bool disposed;

        public VertexAttributeObject()
        {
            Handle = GL.GenVertexArray();
        }

#if WARN_MEMORY_LEAKS
        ~VertexAttributeObject()
        {
            Log.Assert(disposed, $"{nameof(VertexAttributeObject)} was finalized but not disposed, this is a memory leak.", LogType.Warning);
        }
#endif // WARN_MEMORY_LEAKS

        public void Bind()
        {
            GL.BindVertexArray(Handle);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            GL.DeleteVertexArray(Handle);
            GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}
