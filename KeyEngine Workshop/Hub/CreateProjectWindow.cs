using ImGuiNET;
using System.Numerics;
using KeyEngine_Workshop.GUI;
using KeyEngine_Workshop.Core;
using KeyEngine_Workshop.Projects;

namespace KeyEngine_Workshop.Hub
{
    // TODO: Сделать чтобы последний ввёденый Path сохранялся в конфиг
    public class CreateProjectWindow : GuiWindowBase
    {
        public const string WINDOW_DISPLAY_NAME = "Create project";

        private string enteredProjectName = string.Empty;
        private string cachedProjectPath = string.Empty;
        private string validationText = string.Empty;
        private string enteredProjectPath = ProjectManager.ProjectsFolderPath;
        private readonly char[] invalidPathChars = GetPathInvalidChars();
        private bool canUserCreateProject = true;
        private ProjectTemplateInfo currentComboItem
        { 
            get
            {
                return field.Path == null ? ProjectManager.LoadedProjectTemplates.First() : field;
            }
            set => field = value;
        }

        protected override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;

        public CreateProjectWindow()
        {
            Title = WINDOW_DISPLAY_NAME;
            _isVisible = false;
        }

        protected override void PreBegin()
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            Vector2 viewportSize = viewport.Size;

            Vector2 windowSize = new Vector2(300, 150);

            Vector2 centerPos = viewport.Pos + (viewportSize - windowSize) * 0.5f;

            ImGui.SetNextWindowPos(centerPos, ImGuiCond.Always);
            ImGui.SetNextWindowSize(windowSize, ImGuiCond.Always);
        }

        protected override void DrawContent()
        {
            bool projectPathChanged = ImGui.InputText("ProjectPath", ref enteredProjectPath, 260);
            bool projectNameChanged = ImGui.InputText("ProjectName", ref enteredProjectName, 64);

            if (projectNameChanged || projectPathChanged)
            {
                cachedProjectPath = Path.Combine(enteredProjectPath, enteredProjectName);
                ValidateProjectPath();
            }

            DrawProjectTemplate();

            if (!canUserCreateProject)
            { 
                ImGui.TextColored(new(1, 1, 0, 1), validationText);
                ImGui.BeginDisabled();
            }

            if (ImGui.Button("Create"))
            {
                ProjectManager.CreateProject(currentComboItem, enteredProjectName, enteredProjectPath);
                Reset();
            }

            if (!canUserCreateProject)
                ImGui.EndDisabled();

            ImGui.SameLine();

            if (ImGui.Button("Cancel"))
            {
                IsVisible = false;
                Reset();
            }
        }

        private void DrawProjectTemplate()
        {
            IEnumerable<ProjectTemplateInfo> templates = ProjectManager.LoadedProjectTemplates;

            if (ImGui.BeginCombo("Project template", currentComboItem.DisplayName))
            {
                foreach (ProjectTemplateInfo templateInfo in templates)
                {
                    bool isSelected = currentComboItem.DisplayName == templateInfo.DisplayName;

                    if (ImGui.Selectable(templateInfo.DisplayName, isSelected))
                    {
                        currentComboItem = templateInfo;
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }
        }

        private void ValidateProjectPath()
        {
            if (string.IsNullOrEmpty(enteredProjectName))
            {
                validationText = "The project must have a name!";
                canUserCreateProject = false;
                return;
            }

            if (cachedProjectPath.Length > 260)
            {
                validationText = "Path must be less than 260 characters!";
                canUserCreateProject = false;
                return;
            }

            if (!IsPathValid(cachedProjectPath) || cachedProjectPath.Count(ch => ch == ':') > 1)
            {
                validationText = "Project location is invalid!";
                canUserCreateProject = false;
                return;
            }

            if (Directory.Exists(cachedProjectPath))
            {
                validationText = "A project with this name\nalready exists at this location!";
                canUserCreateProject = false;
                return;
            }

            canUserCreateProject = true;
        }

        private bool IsPathValid(string path)
        {
            Path.GetFullPath(path);
            if (string.IsNullOrWhiteSpace(path) || path.IndexOfAny(invalidPathChars) != -1 || path.Length > 260)
            {
                return false;
            }

            return true;
        }

        private static char[] GetPathInvalidChars()
        {
            List<char> pathInvalidChars = Path.GetInvalidPathChars().ToList();
            List<char> fileNameInvalidChars = Path.GetInvalidFileNameChars().ToList();

            List<char> result = [];

            foreach (char ch in fileNameInvalidChars)
            {
                switch (ch)
                {
                    case '/':
                        continue;
                    case '\\':
                        continue;
                    case ':':
                        continue;
                    default:
                        result.Add(ch);
                        continue;
                }
            }

            foreach (char ch in pathInvalidChars)
            {
                switch (ch)
                {
                    case '/':
                        continue;
                    case '\\':
                        continue;
                    case ':':
                        continue;
                    default:
                        if (!result.Contains(ch))
                            result.Add(ch);
                        continue;
                }
            }

            return [.. result];
        }

        private void Reset()
        {
            _isVisible = false;
            enteredProjectName = string.Empty;
            canUserCreateProject = true;
        }
    }
}
