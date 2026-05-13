using KeyEngine_Workshop.GUI;
using KeyEngine_Workshop.Projects;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.ComponentModel;

namespace KeyEngine_Workshop.Windowing
{
    public class ApplicationWindow : GameWindow
    {
        private static ApplicationWindow instance = null!;
        public static ApplicationWindow Instance { get => instance; }
        private bool isRunning;

        private readonly List<IOpenTKWindowHandler> windowHandlers = [];

        private ApplicationWindow(NativeWindowSettings nativeWindowSettings) : base(GameWindowSettings.Default, nativeWindowSettings) { }

        public static void Initialize()
        {
            if (instance != null)
                throw new InvalidOperationException($"{nameof(ApplicationWindow)} already initialized.");

            instance = CreateWindow();

            GL.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);
        }

        public static new void Run()
        {
            if (instance == null)
                throw new NullReferenceException("The window is not initialized.");

            if (Instance.isRunning)
                throw new InvalidOperationException("The window is already running.");

            Instance.isRunning = true;
            ((GameWindow)instance).Run();
        }

        private static ApplicationWindow CreateWindow()
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                Profile = ContextProfile.Core,
                Flags = ContextFlags.Default,
                API = ContextAPI.OpenGL,

                ClientSize = new Vector2i(640, 480),
                Vsync = VSyncMode.On,
                Title = "KeyEngine Workshop",

                WindowState = WindowState.Normal,
                WindowBorder = WindowBorder.Resizable,
                IsEventDriven = true
            };

            return new ApplicationWindow(nativeWindowSettings);
        }

        public IOpenTKWindowHandler RegisterHandler(IOpenTKWindowHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            windowHandlers.Add(handler);
            return handler;
        }

        public void RemoveHandler(IOpenTKWindowHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            windowHandlers.Remove(handler);
        }

        public T? GetHandler<T>() where T : IOpenTKWindowHandler
        {
            if (isRunning)
                foreach (IOpenTKWindowHandler windowHandler in windowHandlers)
                    if (windowHandler is T t)
                        return t;

            return default;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            if (isRunning)
                foreach (IOpenTKWindowHandler windowHandler in windowHandlers)
                    windowHandler.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (isRunning)
                foreach (IOpenTKWindowHandler windowHandler in windowHandlers)
                    windowHandler.OnWindowUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            base.OnRenderFrame(args);

            if (isRunning)
                foreach (IOpenTKWindowHandler windowHandler in windowHandlers)
                    windowHandler.OnWindowRenderFrame(args);

            Context.SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            if (isRunning)
                foreach (IOpenTKWindowHandler windowHandler in windowHandlers)
                    windowHandler.OnWindowResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            if (isRunning)
                foreach (IOpenTKWindowHandler windowHandler in windowHandlers)
                    windowHandler.OnWindowTextInput(e);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            if (isRunning)
                foreach (IOpenTKWindowHandler windowHandler in windowHandlers)
                    windowHandler.OnWindowUnload();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (isRunning)
                foreach (IOpenTKWindowHandler windowHandler in windowHandlers)
                    windowHandler.OnWindowClosing(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (isRunning)
                foreach (IOpenTKWindowHandler windowHandler in windowHandlers)
                    windowHandler.OnWindowMouseWheel(e);
        }
    }
}
