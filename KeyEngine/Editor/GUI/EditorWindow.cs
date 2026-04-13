using ImGuiNET;

namespace KeyEngine.Editor.GUI
{
    public class EditorWindow
    {
        public string Title = "Editor Window";

        public virtual void Render() { }
        public virtual void Begin() { ImGui.Begin(Title); }
        public virtual void End() { ImGui.End(); }
    }
}
