using ImGuiNET;
using KeyEngine.Rendering;
using System.Numerics;

namespace KeyEngine.Editor.GUI.Themes
{
    public class DefaultTheme : ITheme
    {
        private ImFontPtr font;

        ImFontPtr ITheme.Font => font;

        public void Apply(ImGuiController controller)
        {
            font = ImGui.GetIO().Fonts.AddFontFromFileTTF("Editor/Pixel KeyDev font.ttf", 12, null, ImGui.GetIO().Fonts.GetGlyphRangesCyrillic());
            controller.RecreateFontDeviceTexture();

            ImGuiStylePtr style = ImGui.GetStyle();
            RangeAccessor<Vector4> colors = style.Colors;

            colors[(int)ImGuiCol.Text] = new Vector4(0.860f, 0.930f, 0.890f, 0.78f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.860f, 0.930f, 0.890f, 0.28f);
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.13f, 0.14f, 0.17f, 1.00f);
            colors[(int)ImGuiCol.ChildBg] = new Vector4(0.200f, 0.220f, 0.270f, 0.58f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.200f, 0.220f, 0.270f, 0.9f);
            colors[(int)ImGuiCol.Border] = new Vector4(0.31f, 0.31f, 1.00f, 0.00f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.200f, 0.220f, 0.270f, 1.00f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.455f, 0.198f, 0.301f, 0.78f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.232f, 0.201f, 0.271f, 1.00f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.502f, 0.075f, 0.256f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.200f, 0.220f, 0.270f, 0.75f);
            colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.200f, 0.220f, 0.270f, 0.47f);
            colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.200f, 0.220f, 0.270f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.09f, 0.15f, 0.1f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.455f, 0.198f, 0.301f, 0.78f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.CheckMark] = new Vector4(0.71f, 0.22f, 0.27f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.47f, 0.77f, 0.83f, 0.14f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.71f, 0.22f, 0.27f, 1.00f);
            colors[(int)ImGuiCol.Button] = new Vector4(0.47f, 0.77f, 0.83f, 0.14f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.455f, 0.198f, 0.301f, 0.86f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.Header] = new Vector4(0.455f, 0.198f, 0.301f, 0.76f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.455f, 0.198f, 0.301f, 0.86f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.502f, 0.075f, 0.256f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.47f, 0.77f, 0.83f, 0.04f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.455f, 0.198f, 0.301f, 0.78f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.PlotLines] = new Vector4(0.860f, 0.930f, 0.890f, 0.63f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.860f, 0.930f, 0.890f, 0.63f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.455f, 0.198f, 0.301f, 0.43f);
            colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.200f, 0.220f, 0.270f, 0.73f);
            colors[(int)ImGuiCol.Tab] = new Vector4(0.232f, 0.201f, 0.271f, 1.00f);
            colors[(int)ImGuiCol.TabSelected] = new Vector4(0.502f, 0.075f, 0.256f, 1.00f);
            colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.200f, 0.220f, 0.270f, 0.75f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.631f, 0.098f, 0.322f, 0.75f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.26f, 0.29f, 0.38f, 0.90f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.43f, 0.19f, 0.29f, 0.78f);
            colors[(int)ImGuiCol.TabSelectedOverline] = new Vector4(0.98f, 0.26f, 0.26f, 0.00f);
            colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.23f, 0.20f, 0.27f, 1.00f);
            colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.82f, 0.34f, 0.53f, 0.39f);
            colors[(int)ImGuiCol.NavCursor] = new Vector4(0.82f, 0.34f, 0.53f, 1.00f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.45f, 0.20f, 0.30f, 0.78f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.45f, 0.20f, 0.30f, 1.00f);


        }
    }
}
