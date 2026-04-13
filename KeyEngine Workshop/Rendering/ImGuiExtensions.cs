using ImGuiNET;
using System.Numerics;

namespace KeyEngine_Workshop.Rendering
{
    public static class ImGuiExtensions
    {
        extension(ImGui)
        {
            #region ImageUpside
            public static void ImageUpside(nint user_texture_id, Vector2 image_size, Vector4 tint_col, Vector4 border_col)
            {
                ImGui.Image(user_texture_id, image_size, new Vector2(0, 1), new Vector2(1, 0), tint_col, border_col);
            }

            public static void ImageUpside(nint user_texture_id, Vector2 image_size, Vector4 tint_col)
            {
                ImGui.Image(user_texture_id, image_size, new Vector2(0, 1), new Vector2(1, 0), tint_col);
            }

            public static void ImageUpside(nint user_texture_id, Vector2 image_size)
            {
                ImGui.Image(user_texture_id, image_size, new Vector2(0, 1), new Vector2(1, 0));
            }
            #endregion // ImageUpside

            public static void SetCursorOffsetY(float offset)
            {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + offset);
            }

            public static void SetCursorOffsetX(float offset)
            {
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offset);
            }

            public static bool IsItemDoubleClicked(ImGuiMouseButton button)
            {
                return ImGui.IsMouseDoubleClicked(button) && ImGui.IsItemHovered();
            }

            public static bool InspectorVariable(string nameText, string valueName, float valueSizePercentage = 0.5f, float buttonHeight = 10)
            {
                InspectorVariableText(nameText);

                return InspectorVariableValue(nameText, valueSizePercentage, buttonHeight);
            }

            public static void InspectorVariableText(string nameText)
            {
                ImGui.Text(nameText);
                ImGui.SameLine();
            }

            public static bool InspectorVariableValue(string valueName, float valueSizePercentage = 0.5f, float buttonHeight = 10)
            {
                float remaining = ImGui.GetContentRegionAvail().X;
                float buttonWidth = remaining * valueSizePercentage;
                buttonHeight += ImGui.GetItemRectSize().Y;

                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + remaining - buttonWidth);

                return ImGui.Button(valueName, new Vector2(buttonWidth, buttonHeight));
            }
        }
    }
}
