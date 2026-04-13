using ImGuiNET;
using KeyEngine.Rendering;
using KeyEngine.Editor.GUI;
using OpenTK.Windowing.Common;
using KeyEngine.Editor.GUI.Themes;
using KeyEngine.Editor.GUI.Windows;
using KeyEngine.Editor.GUI.Inspector;
using KeyEngine.Editor.GUI.FileBrowser;
using KeyEngine.Editor.GUI.AssetEditor;

namespace KeyEngine.Editor.Systems
{
    public class EditorGuiSystem : EditorSystem
    {
        private readonly ImGuiController imGuiController;
        private static readonly List<EditorWindow> editorWindows = [];
        private ITheme currentTheme;

        public static bool EnableRenderingGUI { get; set; } = true;
        public static bool IsMouseOnGUI { get; private set; }

        private static void RegisterWindows()
        {
            RegisterWindow<HelloWindow>();
            RegisterWindow<Inspector>();
            RegisterWindow<Hierarchy>();
            RegisterWindow<FileBrowser>();
            RegisterWindow<PlaybackStateWindow>();
            RegisterWindow<DebugInfo>();
            RegisterWindow<ConsoleWindow>();
            RegisterWindow<AssetEditorWindow>();
        }

        public EditorGuiSystem()
        {
            imGuiController = new ImGuiController(MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y);

            MainWindow.Instance.TextInput += OnTextInput;
            MainWindow.Instance.Resize += OnResized;
            MainWindow.Instance.MouseWheel += OnMouseWheel;

            RegisterWindows();

            // Delete
            currentTheme = new DefaultTheme();
            currentTheme.Apply(imGuiController);

            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            io.ConfigViewportsNoAutoMerge = true;
            io.ConfigViewportsNoTaskBarIcon = false;
        }

        public override void Update(float deltaTime)
        {
            imGuiController.Update(MainWindow.Instance, deltaTime);
        }

        public override void Render()
        {
            if (!EnableRenderingGUI)
                return;

            ImGui.ShowStyleEditor();
            ImGui.PushFont(currentTheme.Font);

            ImGui.DockSpaceOverViewport(0, ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode | 
                ImGuiDockNodeFlags.NoDockingOverCentralNode);

            bool anyWindowHovered = false;

            for (int i = 0; i < editorWindows.Count; i++)
            {
                EditorWindow window = editorWindows[i];

                window.Begin();
                window.Render();
                if (IsAnyWindowHovered())
                {
                    if (anyWindowHovered == false)
                        anyWindowHovered = true;
                }
                window.End();
            }

            ImGui.PopFont();

            IsMouseOnGUI = anyWindowHovered;
            imGuiController.Render();

            ImGui.UpdatePlatformWindows();
            ImGui.RenderPlatformWindowsDefault();
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

        private static bool IsAnyWindowHovered() => 
            ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) ||
            ImGui.IsAnyItemHovered() ||
            ImGui.IsAnyItemFocused() ||
            ImGui.IsPopupOpen(null, ImGuiPopupFlags.AnyPopup);

        public static void RegisterWindow<T>() where T : EditorWindow
        {
            object? instance = Activator.CreateInstance(typeof(T));

            if (instance != null)
                editorWindows.Add((EditorWindow)instance);
        }
    }
}
