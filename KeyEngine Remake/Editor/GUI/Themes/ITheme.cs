using ImGuiNET;
using KeyEngine.Rendering;

namespace KeyEngine.Editor.GUI
{
    public interface ITheme
    {
        public ImFontPtr Font { get; }
        public void Apply(ImGuiController controller);
    }
}
