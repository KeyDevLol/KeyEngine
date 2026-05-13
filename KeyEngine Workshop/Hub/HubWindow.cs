using ImGuiNET;
using System.Numerics;
using System.Reflection;
using KeyEngine_Workshop.GUI;
using KeyEngine_Workshop.Core;
using KeyEngine_Workshop.Themes;
using KeyEngine_Workshop.Projects;
using KeyEngine_Workshop.Rendering;
using KeyEngine_Workshop.Windowing;

namespace KeyEngine_Workshop.Hub
{
    public class HubWindow : GuiWindowBase
    {
        private static HubWindow instance = null!;

        private readonly Texture KeyEngineLogo = null!;
        private readonly Texture MascotBackground = null!;
        private readonly ITheme currentTheme = new DefaultTheme();
        private readonly string VersionStr = string.Empty;
        private ImGuiViewportPtr viewport = null!;

        private HubWindow()
        {
            if (instance != null)
                throw new InvalidOperationException($"{nameof(HubWindow)} instance already exists.");

            WindowFlags = ImGuiWindowFlags.NoDecoration |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoSavedSettings |
            ImGuiWindowFlags.NoBringToFrontOnFocus;
            Title = "HubWindow";

            ApplicationWindow mainWindow = ApplicationWindow.Instance;

            KeyEngineLogo = new Texture("Assets/KeyEngineLogo.png");
            MascotBackground = new Texture("Assets/Mascot.png");

            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version ?? throw new NullReferenceException("appVersion is null.");
            VersionStr = $"v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
        }

        public override void OnRegister()
        {
            viewport = ImGui.GetMainViewport();
            currentTheme.Apply(WindowManager.GetInstance().ImGuiController);
        }

        //public override void OnWindowClosing(CancelEventArgs args)
        //{
        //    ProjectManager.SaveProjectList();
        //}

        protected override void PreBegin()
        {
            ImGui.DockSpaceOverViewport(0, viewport,
    ImGuiDockNodeFlags.PassthruCentralNode |
    ImGuiDockNodeFlags.NoDockingOverCentralNode);

            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
        }

        protected override void DrawContent()
        {
            OpenTK.Mathematics.Vector2i mainWindowSize = ApplicationWindow.Instance.ClientSize;
            Vector2 windowSize = new Vector2(mainWindowSize.X, mainWindowSize.Y);

            ImGui.PushFont(currentTheme.Font);

            DrawMascotBackground(new Vector2((ImGui.GetWindowWidth() / 3f), 50), 0.8f);

            float x = windowSize.X;
            float y = windowSize.Y;

            DrawUpTab();
            DrawProjectsWindow();

            //ImGui.End(); // Main
            ImGui.PopFont();
        }

        private void DrawMascotBackground(Vector2 padding = default, float scale = 1.0f)
        {
            var windowPos = ImGui.GetWindowPos();
            var windowSize = ImGui.GetWindowSize();

            float textureWidth = MascotBackground.Width;
            float textureHeight = MascotBackground.Height;

            float windowAspect = windowSize.X / windowSize.Y;
            float textureAspect = textureWidth / textureHeight;

            Vector2 baseSize;
            if (windowAspect > textureAspect)
            {
                baseSize.Y = windowSize.Y;
                baseSize.X = windowSize.Y * textureAspect;
            }
            else
            {
                baseSize.X = windowSize.X;
                baseSize.Y = windowSize.X / textureAspect;
            }

            var drawSize = baseSize * scale;

            var drawPos = windowPos + (windowSize - drawSize) * 0.5f;

            drawPos.X += padding.X;
            drawPos.Y += padding.Y;

            var p1 = drawPos;
            var p2 = new Vector2(drawPos.X + drawSize.X, drawPos.Y);
            var p3 = drawPos + drawSize;
            var p4 = new Vector2(drawPos.X, drawPos.Y + drawSize.Y);

            var uv0 = new Vector2(0, 1);
            var uv1 = new Vector2(1, 1);
            var uv2 = new Vector2(1, 0);
            var uv3 = new Vector2(0, 0);

            ImGui.GetWindowDrawList().AddImageQuad(
                MascotBackground.Handle,
                p1, p2, p3, p4,
                uv0, uv1, uv2, uv3, ImGui.GetColorU32(ImGuiCol.Tab)
            );
        }

        private void DrawUpTab()
        {
            ImGui.ImageUpside(KeyEngineLogo.Handle, new Vector2(KeyEngineLogo.Width, KeyEngineLogo.Height));
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 38);
            ImGui.SetCursorOffsetY(8);
            ImGui.ImageUpside(Texture.Square.Handle, new Vector2(32));
            ImGui.Text($"KeyEngine Workshop {VersionStr}");
            ImGui.Dummy(new Vector2(0, 5));
            ImGui.Separator();
        }

        private void DrawProjectsWindow()
        {
            if (ImGui.BeginChild("ProjectsWindow"))
            {
                if (ImGui.Button("Create##CreateNewProject", new Vector2(64, 28)))
                    WindowManager.GetInstance().GetWindow<CreateProjectWindow>(CreateProjectWindow.WINDOW_DISPLAY_NAME).IsVisible = true;

                ImGui.SameLine();
                ImGui.Button("Import##ImportNewProject", new Vector2(64, 28));
                ImGui.Separator();
                DrawProjectList();
                ImGui.EndChild();
            }
        }

        private void DrawProjectList()
        {
            Vector2 contentRegion = ImGui.GetContentRegionAvail();
            Vector2 listSize = new Vector2(contentRegion.X / 1.5f, contentRegion.Y / 1.1f);

            if (ImGui.BeginListBox("##ProjectList", listSize))
            {
                foreach (Project project in ProjectManager.LoadedProjects)
                {
                    DirectoryInfo directoryInfo = new(project.Path);
                    string fullPath = Path.GetFullPath(project.Path);
                    ImGui.Selectable($"{directoryInfo.Name}\n{fullPath}", false, ImGuiSelectableFlags.None, new Vector2(listSize.X / 1.1f, 40));
                    if (ImGui.IsItemHovered())
                        ImGui.SetTooltip(fullPath);
                    ImGui.SameLine();
                    ImGui.Button($"...##{project.Path}");
                    ImGui.Separator();
                }
                ImGui.EndListBox();
            }
        }

        public static HubWindow GetInstance()
        {
            instance ??= new HubWindow();
            return instance;
        }
    }
}
