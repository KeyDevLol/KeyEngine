using ImGuiNET;
using KeyEngine.Mathematics;

namespace KeyEngine.Editor.GUI
{
    public class DebugInfo : EditorWindow
    {
        private readonly double[] fps = new double[100];
        private double time;

        private double averageFPS;
        private int fpsIndex;

        public DebugInfo()
        {
            title = "Debug Info";
        }

        public override void Render()
        {
            double ms = (ImGui.GetTime() - time);
            time = ImGui.GetTime();

            fps[fpsIndex] = Mathf.Round((float)(1f / ms));

            ImGui.Text($"MS: {(ms * 1000):F2}");
            ImGui.Text($"FPS: {(1f / ms):F0}");
            ImGui.Text($"AVRG FPS: {averageFPS:F1}");
            ImGui.Text($"Total Objects: {ECS.EntitiesCount}");

            fpsIndex++;

            if (fpsIndex > fps.Length - 1)
            {
                fpsIndex = 0;
                averageFPS = fps.Average();
            }
        }
    }
}
