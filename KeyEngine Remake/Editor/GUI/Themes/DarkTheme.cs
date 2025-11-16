using ImGuiNET;
using KeyEngine.Rendering;
using System.Numerics;

namespace KeyEngine.Editor.GUI.Themes
{
    internal class DarkTheme : ITheme
    {
        private ImFontPtr font;
        ImFontPtr ITheme.Font => font;

        public void Apply(ImGuiController controller)
        {
            font = ImGui.GetIO().Fonts.AddFontFromFileTTF("Editor/Pixel KeyDev font.ttf", 12, null, ImGui.GetIO().Fonts.GetGlyphRangesCyrillic());
            controller.RecreateFontDeviceTexture();

            var style = ImGui.GetStyle();
            var colors = style.Colors;
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.07f, 0.07f, 0.09f, 1.00f);
            colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.12f, 0.12f, 0.15f, 1.00f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.18f, 0.18f, 0.22f, 1.00f);
            colors[(int)ImGuiCol.Header] = new Vector4(0.18f, 0.18f, 0.22f, 1.00f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.30f, 0.30f, 0.40f, 1.00f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.25f, 0.25f, 0.35f, 1.00f);
            colors[(int)ImGuiCol.Button] = new Vector4(0.20f, 0.22f, 0.27f, 1.00f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.30f, 0.32f, 0.40f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.35f, 0.38f, 0.50f, 1.00f);
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.15f, 0.15f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.22f, 0.22f, 0.27f, 1.00f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.25f, 0.25f, 0.30f, 1.00f);
            colors[(int)ImGuiCol.Tab] = new Vector4(0.18f, 0.18f, 0.22f, 1.00f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.35f, 0.35f, 0.50f, 1.00f);
            colors[(int)ImGuiCol.TabDimmed] = new Vector4(0.25f, 0.25f, 0.38f, 1.00f);
            colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.13f, 0.13f, 0.17f, 1.00f);
            colors[(int)ImGuiCol.TabDimmedSelected] = new Vector4(0.20f, 0.20f, 0.25f, 1.00f);
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.12f, 0.12f, 0.15f, 1.00f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.15f, 0.15f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.10f, 0.10f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.Border] = new Vector4(0.20f, 0.20f, 0.25f, 0.50f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.Text] = new Vector4(0.90f, 0.90f, 0.95f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.55f, 1.00f);
            colors[(int)ImGuiCol.CheckMark] = new Vector4(0.50f, 0.70f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.50f, 0.70f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.60f, 0.80f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.50f, 0.70f, 1.00f, 0.50f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.60f, 0.80f, 1.00f, 0.75f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.70f, 0.90f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.10f, 0.10f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.30f, 0.30f, 0.35f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.40f, 0.40f, 0.50f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.45f, 0.45f, 0.55f, 1.00f);
            style.WindowRounding = 5.0f;
            style.FrameRounding = 5.0f;
            style.GrabRounding = 5.0f;
            style.TabRounding = 5.0f;
            style.PopupRounding = 5.0f;
            style.ScrollbarRounding = 5.0f;
            style.WindowPadding = new Vector2(10, 10);
            style.FramePadding = new Vector2(6, 4);
            style.ItemSpacing = new Vector2(8, 6);
            style.PopupBorderSize = 0.0f;
        }
    }
}
