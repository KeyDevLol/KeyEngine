using ImGuiNET;

namespace KeyEngine.Editor.GUI
{
    public class DefaultTheme : ITheme
    {
        public void Apply()
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            var colors = style.Colors;

            colors[(int)ImGuiCol.Text] = new System.Numerics.Vector4(0.860f, 0.930f, 0.890f, 0.78f);
            colors[(int)ImGuiCol.TextDisabled] = new System.Numerics.Vector4(0.860f, 0.930f, 0.890f, 0.28f);
            colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.13f, 0.14f, 0.17f, 1.00f);
            colors[(int)ImGuiCol.ChildBg] = new System.Numerics.Vector4(0.200f, 0.220f, 0.270f, 0.58f);
            colors[(int)ImGuiCol.PopupBg] = new System.Numerics.Vector4(0.200f, 0.220f, 0.270f, 0.9f);
            colors[(int)ImGuiCol.Border] = new System.Numerics.Vector4(0.31f, 0.31f, 1.00f, 0.00f);
            colors[(int)ImGuiCol.BorderShadow] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.200f, 0.220f, 0.270f, 1.00f);
            colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 0.78f);
            colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.TitleBg] = new System.Numerics.Vector4(0.232f, 0.201f, 0.271f, 1.00f);
            colors[(int)ImGuiCol.TitleBgActive] = new System.Numerics.Vector4(0.502f, 0.075f, 0.256f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new System.Numerics.Vector4(0.200f, 0.220f, 0.270f, 0.75f);
            colors[(int)ImGuiCol.MenuBarBg] = new System.Numerics.Vector4(0.200f, 0.220f, 0.270f, 0.47f);
            colors[(int)ImGuiCol.ScrollbarBg] = new System.Numerics.Vector4(0.200f, 0.220f, 0.270f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.09f, 0.15f, 0.1f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 0.78f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(0.71f, 0.22f, 0.27f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab] = new System.Numerics.Vector4(0.47f, 0.77f, 0.83f, 0.14f);
            colors[(int)ImGuiCol.SliderGrabActive] = new System.Numerics.Vector4(0.71f, 0.22f, 0.27f, 1.00f);
            colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(0.47f, 0.77f, 0.83f, 0.14f);
            colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 0.86f);
            colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.Header] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 0.76f);
            colors[(int)ImGuiCol.HeaderHovered] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 0.86f);
            colors[(int)ImGuiCol.HeaderActive] = new System.Numerics.Vector4(0.502f, 0.075f, 0.256f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip] = new System.Numerics.Vector4(0.47f, 0.77f, 0.83f, 0.04f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 0.78f);
            colors[(int)ImGuiCol.ResizeGripActive] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.PlotLines] = new System.Numerics.Vector4(0.860f, 0.930f, 0.890f, 0.63f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram] = new System.Numerics.Vector4(0.860f, 0.930f, 0.890f, 0.63f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 1.00f);
            colors[(int)ImGuiCol.TextSelectedBg] = new System.Numerics.Vector4(0.455f, 0.198f, 0.301f, 0.43f);
            colors[(int)ImGuiCol.ModalWindowDimBg] = new System.Numerics.Vector4(0.200f, 0.220f, 0.270f, 0.73f);
            colors[(int)ImGuiCol.Tab] = new System.Numerics.Vector4(0.232f, 0.201f, 0.271f, 1.00f);
            colors[(int)ImGuiCol.TabActive] = new System.Numerics.Vector4(0.502f, 0.075f, 0.256f, 1.00f);
            colors[(int)ImGuiCol.TabUnfocused] = new System.Numerics.Vector4(0.200f, 0.220f, 0.270f, 0.75f);
            colors[(int)ImGuiCol.TabHovered] = new System.Numerics.Vector4(0.631f, 0.098f, 0.322f, 0.75f);
            colors[(int)ImGuiCol.TabUnfocusedActive] = new System.Numerics.Vector4(0.200f, 0.220f, 0.270f, 0.75f);
        }
    }
}
