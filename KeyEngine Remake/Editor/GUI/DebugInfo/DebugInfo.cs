using ImGuiNET;
using KeyEngine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Editor.GUI
{
    public class DebugInfo : EditorWindow
    {
        private double[] fps = new double[100];
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

            ImGui.Text($"MS: {(ms * 1000).ToString("F2")}");
            ImGui.Text($"FPS: {(1f / ms).ToString("F0")}");
            ImGui.Text($"AVRG FPS: {averageFPS.ToString("F1")}");
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
