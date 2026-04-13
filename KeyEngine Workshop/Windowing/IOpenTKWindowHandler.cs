using OpenTK.Windowing.Common;
using System.ComponentModel;

namespace KeyEngine_Workshop.Windowing
{
    public interface IOpenTKWindowHandler
    {
        void OnWindowUpdateFrame(FrameEventArgs args);
        void OnWindowRenderFrame(FrameEventArgs args);
        void OnWindowResize(ResizeEventArgs args);
        void OnWindowTextInput(TextInputEventArgs args);
        void OnWindowMouseWheel(MouseWheelEventArgs args);
        void OnWindowClosing(CancelEventArgs e);
        void OnWindowUnload();
        void OnLoad();
    }
}
