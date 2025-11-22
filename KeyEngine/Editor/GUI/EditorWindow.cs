using ImGuiNET;

namespace KeyEngine.Editor.GUI
{
    public class EditorWindow
    {
        public string title = "Editor Window";

        public virtual void Render() { }
        public virtual void Begin() { ImGui.Begin(title); }
        public virtual void End() { ImGui.End(); }
    }
}
