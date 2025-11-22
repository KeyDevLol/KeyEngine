using KeyEngine.Audio;
using KeyEngine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KeyEngine
{
    public class MainWindow : GameWindow
    {
        private static MainWindow instance = null!;
        public static MainWindow Instance { get => instance; }
        public static float DeltaTime { get; private set; }

        private MainWindow(NativeWindowSettings nativeWindowSettings) : base(GameWindowSettings.Default, nativeWindowSettings)
        {

        }

        public static void Initialize(IScene startScene)
        {
            if (instance != null)
                throw new InvalidOperationException("MainWindow already initialized.");

            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(640, 480),
                Profile = ContextProfile.Core,
                Flags = ContextFlags.ForwardCompatible,
                API = ContextAPI.OpenGL,

                Vsync = Application.VSync ? VSyncMode.On : VSyncMode.Off,
                Title = Application.WindowTitle,
                WindowState = (WindowState)Application.WindowState,
                WindowBorder = (WindowBorder)Application.WindowBorder,
                NumberOfSamples = Application.MsaaEnabled ? (int)Application.MsaaSamplesCount : 0
            };

            instance = new MainWindow(nativeWindowSettings);
            GL.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);

            SceneManager.LoadScene(startScene, false);
            instance.Run();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused && !Application.RunInBackground)
                return;

            float deltaTime = (float)args.Time;
            DeltaTime = deltaTime;

            PhysicsManager.Update(deltaTime);
            ECS.CallUpdate(deltaTime);
#if ENABLE_EDITOR
            Editor.Editor.Update(deltaTime);
#endif

            Title = $"KeyEngine IV Remake by KeyDev {Math.Round(1 / deltaTime)}";
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (!IsFocused && !Application.RunInBackground)
                return;

            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Render
            if (Camera.Main != null)
            {
                ECS.CallRender();
#if ENABLE_EDITOR
                Editor.Editor.Render();
#endif
            }

            Context.SwapBuffers();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            base.OnFocusedChanged(e);
            //AudioManager.SetPause(!e.IsFocused);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}