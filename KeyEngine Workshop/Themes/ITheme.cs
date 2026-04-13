using ImGuiNET;
using KeyEngine_Workshop.Rendering;

namespace KeyEngine_Workshop.Themes
{
    public interface ITheme
    {
        public ImFontPtr Font { get; }
        public void Apply(ImGuiController controller);
    }
}
