using System.ComponentModel;
using OpenTK.Windowing.Common;

namespace KeyEngine_Workshop.Windowing
{
    public class OpenTKWindowHandlerBase : IOpenTKWindowHandler
    {
        public virtual void OnWindowRenderFrame(FrameEventArgs args) { }
        public virtual void OnWindowUpdateFrame(FrameEventArgs args) { }
        public virtual void OnWindowClosing(CancelEventArgs e) { }
        public virtual void OnWindowUnload() { }
        public virtual void OnWindowResize(ResizeEventArgs args) { }
        public virtual void OnWindowTextInput(TextInputEventArgs args) { }
        public virtual void OnWindowMouseWheel(MouseWheelEventArgs args) { }
        public virtual void OnLoad() { }
    }
}
