using ImGuiNET;
using System.Numerics;

namespace KeyEngine.Rendering
{
    public static class ImGuiExtensions
    {
        extension(ImGui)
        {
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
