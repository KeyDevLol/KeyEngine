﻿using KeyEngine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KeyEngine
{
    public class MainWindow : GameWindow
    {
        private static MainWindow instance = null!;
        public static MainWindow Instance { get => instance; }

        private MainWindow(NativeWindowSettings nativeWindowSettings) : base(GameWindowSettings.Default, nativeWindowSettings) 
        {

        }

        public static void Initialize(IScene startScene)
        {
            if (instance != null)
                return;

            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new OpenTK.Mathematics.Vector2i(640, 480),
                Profile = ContextProfile.Core,
                Flags = ContextFlags.ForwardCompatible,
                API = ContextAPI.OpenGL,
                Vsync = VSyncMode.On,
                Title = "KeyEngine window",
            };

            instance = new MainWindow(nativeWindowSettings);
            GL.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);
            instance.WindowState = WindowState.Normal;
            SceneManager.LoadScene(startScene);

            instance.Run();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (IsFocused == false)
                return;

            float deltaTime = (float)args.Time;

            PhysicsManager.Update(deltaTime);
            ECS.CallUpdate(deltaTime);
#if ENABLE_EDITOR
            Editor.Editor.Update(deltaTime);
#endif

            Title = $"KeyEngine IV Remake {Math.Round(1 / deltaTime)}";
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (IsFocused)
            {
                base.OnRenderFrame(args);

                GL.Clear(ClearBufferMask.ColorBufferBit);

                //Render
                if (Camera.Main != null)
                {
                    ECS.CallRender();
#if ENABLE_EDITOR
                    Editor.Editor.Render();
#endif
                }

                Context.SwapBuffers();
            }
            else
            {
                //Sleeping ZzZ
                GLFW.WaitEvents();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
