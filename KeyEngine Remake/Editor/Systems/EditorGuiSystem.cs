using ImGuiNET;
using KeyEngine.Editor.GUI.Windows;
using KeyEngine.Editor.GUI;
using KeyEngine.Rendering;
using OpenTK.Windowing.Common;

namespace KeyEngine.Editor.Systems
{
    public class EditorGuiSystem : EditorSystem
    {
        private readonly ImGuiController imGuiController;
        private static readonly List<EditorWindow> editorWindows = new List<EditorWindow>();

        public static bool IsMouseOnGUI { get; private set; }

        private static void RegisterWindows()
        {
            RegisterWindow<HelloWindow>();
            RegisterWindow<Inspector>();
            RegisterWindow<Hierarchy>();
            RegisterWindow<FileBrowser>();
            RegisterWindow<PlaybackStateWindow>();
        }

        public EditorGuiSystem()
        {
            imGuiController = new ImGuiController(MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y);

            MainWindow.Instance.TextInput += OnTextInput;
            MainWindow.Instance.Resize += OnResized;
            MainWindow.Instance.MouseWheel += OnMouseWheel;

            RegisterWindows();

            // Delete
            new DefaultTheme().Apply();
        }

        public override void Update(float deltaTime)
        {
            imGuiController.Update(MainWindow.Instance, deltaTime);
        }

        public override void Render()
        {
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode | 
                ImGuiDockNodeFlags.NoDockingInCentralNode);

            bool anyWindowHovered = false;

            for (int i = 0; i < editorWindows.Count; i++)
            {
                EditorWindow window = editorWindows[i];

                window.Begin();
                window.Render();
                if (ImGui.IsWindowHovered() || ImGui.IsAnyItemHovered())
                {
                    if (anyWindowHovered == false)
                    {
                        anyWindowHovered = true;
                    }
                }
                window.End();
            }

            IsMouseOnGUI = anyWindowHovered;
            imGuiController.Render();
        }

        private void OnResized(ResizeEventArgs args)
        {
            imGuiController.WindowResized(args.Width, args.Height);
        }

        private void OnTextInput(TextInputEventArgs args)
        {
            imGuiController.PressChar((char)args.Unicode);
        }
        private void OnMouseWheel(MouseWheelEventArgs args)
        {
            imGuiController.MouseScroll(args.Offset);
        }

        private static bool IsAnyWindowHovered()
        {
            return ImGui.IsWindowHovered() || ImGui.IsAnyItemHovered();
        }

        public static void RegisterWindow<T>() where T : EditorWindow
        {
            object? instance = Activator.CreateInstance(typeof(T));

            if (instance != null)
                editorWindows.Add((EditorWindow)instance);
        }
    }
}
