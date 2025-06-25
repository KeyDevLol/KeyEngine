
namespace KeyEngine.Rendering
{
    public interface IBuffer : IDisposable
    {
        public int Handle { get; }

        public void Bind();

        public void Unbind();
    }
}
