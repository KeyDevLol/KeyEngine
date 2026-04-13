//using KeyEngine_Workshop.Rendering;
//using KeyEngine_Workshop.Windowing;
//using OpenTK.Windowing.Common;

//namespace KeyEngine_Workshop.GUI
//{
//    public class ImGuiWindowHandler : OpenTKWindowHandlerBase
//    {
//        public readonly ImGuiController ImGuiController = new(ApplicationWindow.Instance.ClientSize.X, ApplicationWindow.Instance.ClientSize.Y);

//        public override void OnWindowUpdateFrame(FrameEventArgs args)
//        {
//            ImGuiController.Update(ApplicationWindow.Instance, (float)args.Time);
//        }

//        public override void OnWindowRenderFrame(FrameEventArgs args)
//        {
//            ImGuiController.Render();
//        }

//        public override void OnWindowMouseWheel(MouseWheelEventArgs args)
//        {
//            ImGuiController.MouseScroll(args.Offset);
//        }

//        public override void OnWindowTextInput(TextInputEventArgs args)
//        {
//            ImGuiController.PressChar((char)args.Unicode);
//        }

//        public override void OnWindowResize(ResizeEventArgs args)
//        {
//            ImGuiController.WindowResized(args.Width, args.Height);
//        }
//    }
//}
