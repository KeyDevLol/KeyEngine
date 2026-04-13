using OpenTK.Windowing.Common;
using KeyEngine_Workshop.Windowing;
using KeyEngine_Workshop.Hub;
using KeyEngine_Workshop.Rendering;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace KeyEngine_Workshop.GUI
{
    public class WindowManager : OpenTKWindowHandlerBase
    {
        private static WindowManager instance = null!;
        private readonly List<GuiWindowBase> windowList = [];
        private readonly Dictionary<string, GuiWindowBase> windowDict = [];
        public readonly ImGuiController ImGuiController;

        private WindowManager()
        {
            if (instance != null)
                throw new InvalidOperationException($"{nameof(WindowManager)} instance is already exists.");

            Vector2i clientSize = ApplicationWindow.Instance.ClientSize;
            ImGuiController = new ImGuiController(clientSize.X, clientSize.Y);
        }

        public override void OnLoad()
        {
            RegisterDefaultWindows();
        }

        public void RegisterWindow(GuiWindowBase window)
        {
            window.OnRegister();
            windowList.Add(window);
            windowDict.Add(window.Title, window);
        }

        private void RegisterDefaultWindows()
        {
            RegisterWindow(HubWindow.GetInstance());
            //RegisterWindow(new TestGui());
        }

        public T? GetWindow<T>(string title) where T : GuiWindowBase
        {
            windowDict.TryGetValue(title, out GuiWindowBase? result);

            return result as T;
        }

        public override void OnWindowRenderFrame(FrameEventArgs args)
        {
            var size = ApplicationWindow.Instance.ClientSize;
            GL.Viewport(0, 0, size.X, size.Y);

            foreach (GuiWindowBase window in windowList)
                window.OnRenderFrame();

            ImGuiController.Render();
        }


        public override void OnWindowUpdateFrame(FrameEventArgs args)
        {
            foreach (GuiWindowBase window in windowList)
            {
                window.OnUpdateFrame((float)args.Time);
            }

            ImGuiController.Update(ApplicationWindow.Instance, (float)args.Time);
        }

        public override void OnWindowMouseWheel(MouseWheelEventArgs args)
        {
            ImGuiController.MouseScroll(args.Offset);
        }

        public override void OnWindowTextInput(TextInputEventArgs args)
        {
            ImGuiController.PressChar((char)args.Unicode);
        }

        public override void OnWindowResize(ResizeEventArgs args)
        {
            ImGuiController.WindowResized(args.Width, args.Height);
        }

        public static WindowManager GetInstance()
        {
            instance ??= new();
            return instance;
        }
    }
}
