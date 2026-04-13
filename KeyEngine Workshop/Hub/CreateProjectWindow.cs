using ImGuiNET;
using KeyEngine_Workshop.Projects;
using KeyEngine_Workshop.Windowing;
using OpenTK.Windowing.Common;

namespace KeyEngine_Workshop.Hub
{
    public class CreateProjectWindow : OpenTKWindowHandlerBase
    {
        public const string WINDOW_DISPLAY_NAME = "Create project##CreateProjectWindow";

        private string enteredProjectName = string.Empty;
        private int skipFrames = 0;
        private readonly char[] invalidPathChars = Path.GetInvalidPathChars();

        public void OpenPopup()
        {
            skipFrames = 0;
            ImGui.OpenPopup(WINDOW_DISPLAY_NAME);
        }

        public override void OnWindowRenderFrame(FrameEventArgs args)
        {
            if (ImGui.BeginPopupModal(WINDOW_DISPLAY_NAME))
            {
                bool projectNameChanged = ImGui.InputText("ProjectName", ref enteredProjectName, 64);

                // Проверка Any для того, чтобы не создавать новую строчку каждый кадр, если даже строка не была изменена.
                if (projectNameChanged && enteredProjectName.Any(c => invalidPathChars.Contains(c)))
                {
                    enteredProjectName = string.Concat(enteredProjectName.Where(c => !invalidPathChars.Contains(c)));
                }

                if (ImGui.Button("Create"))
                {
                    ProjectManager.CreateProject(new ProjectTemplate("ProjectTemplates"), enteredProjectName, "CreatedProjects");
                    ImGui.CloseCurrentPopup();
                    enteredProjectName = string.Empty;
                }

                ImGui.SameLine();

                if (ImGui.Button("Cancel"))
                {
                    ClosePopup();
                }

                if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && !ImGui.IsWindowHovered() && skipFrames > 5)
                    ClosePopup();

                ImGui.EndPopup();

                //ImGui.SameLine();
                //ImGui.BeginListBox("##ProjectTemplatesListBox");
                //ImGui.Selectable("Penis");
                //ImGui.EndListBox();

                skipFrames++;
            }
        }

        private void ClosePopup()
        {
            ImGui.CloseCurrentPopup();
            skipFrames = 0;
        }
    }
}
