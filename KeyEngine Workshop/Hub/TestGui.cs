using ImGuiNET;
using KeyEngine_Workshop.GUI;
using KeyEngine_Workshop.Windowing;
using OpenTK.Windowing.Common;

namespace KeyEngine_Workshop.Hub
{
    public class TestGui : GuiWindowBase
    {
        public TestGui()
        {
            Title = "TestWindow";
        }

        protected override void DrawContent()
        {
            ImGui.Button("Hello World Button!");
        }
    }
}
