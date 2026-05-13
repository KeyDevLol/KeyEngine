using ImGuiNET;
using KeyEngine.Mathematics;
using System.Runtime.InteropServices;

namespace KeyEngine.Editor.GUI
{
    public unsafe class DebugInfo : EditorWindow
    {
        private byte* pPlotText;
        private nint pPlotTextH;

        private float[] fps = new float[500];
        private double time;

        private double averageFPS;
        private int fpsIndex;

        public DebugInfo()
        {
            Title = "Debug Info";

            pPlotTextH = Marshal.StringToHGlobalAnsi("PULSE");
            pPlotText = (byte*)pPlotTextH;
        }

        ~DebugInfo()
        {
            Marshal.FreeHGlobal(pPlotTextH);
        }

        public override unsafe void Render()
        {
            double ms = (ImGui.GetTime() - time);
            time = ImGui.GetTime();

            fps[fpsIndex] = Mathf.Round((float)(1f / ms));

            ImGui.Text($"FPS: {(1f / ms):F0}");
            ImGui.Text($"AVG FPS: {averageFPS:F1}");
            ImGui.Text($"Total Objects: {ECS.EntitiesCount}");

            fixed (float* data = fps)
            {
                int values_offset = 0;
                byte* overlay_text = null;
                float scale_min = float.MaxValue;
                float scale_max = float.MaxValue;
                Vector2 graph_size = new Vector2(100, 50);
                int stride = 4;
                ImGuiNative.igPlotLines_FloatPtr(pPlotText, data, fps.Length, values_offset, overlay_text, scale_min, scale_max, graph_size, stride);
            }

            fpsIndex++;

            if (fpsIndex > fps.Length - 1)
            {
                fpsIndex = 0;
                averageFPS = fps.Average();
            }
        }
    }
}
