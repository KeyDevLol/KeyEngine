using ImGuiNET;
using System.Numerics;

namespace KeyEngine.Editor.SupportedTypes
{
    // TODO: Сделать, чтобы текст был выше кнопки выбора
    public class EnumTypeSupport : TypeSupport
    {
        private bool popupOpened;
        private string? searchInput = string.Empty;
        private string? cacheDisplayName;

        public override object Render(TypeSupportRenderArgs args)
        {
            popupOpened = cacheDisplayName == args.DisplayName;

            Type enumType = args.Value!.GetType();
            string[] enumNames = Enum.GetNames(enumType);

            object value = args.Value!;

            if (DrawInspectorVariable(args.VariableName, args.VariableId, value.ToString()!, out float windowWidth, out Vector2 windowPos))
            {
                if (popupOpened)
                {
                    ClosePopup();
                }
                else
                {
                    cacheDisplayName = args.DisplayName;
                    ImGui.OpenPopup(args.DisplayName);
                }
            }

            if (popupOpened)
            {
                ImGuiViewportPtr viewport = ImGui.GetMainViewport();

                ImGui.SetNextWindowSizeConstraints(Vector2.Zero, new Vector2(windowWidth, viewport.WorkSize.Y - windowPos.Y));
                ImGui.SetNextWindowPos(windowPos);

                DrawPopup(args, enumType, enumNames, ref value);
            }

            return value;
        }

        private void DrawPopup(TypeSupportRenderArgs args, Type enumType, string[] enumNames, ref object value)
        {
            if (ImGui.BeginPopup(args.DisplayName, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.Popup))
            {
                ImGui.InputTextWithHint("##Search", "Search...", ref searchInput, 60, ImGuiInputTextFlags.NoHorizontalScroll);

                string[]? keyCodes = null;

                if (!string.IsNullOrEmpty(searchInput))
                {
                    StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
                    string lowerSearchInput = searchInput.ToLower();
                    keyCodes = enumNames!.Where(s => s.StartsWith(lowerSearchInput, comparison) || s.Equals(lowerSearchInput, comparison)).ToArray();
                }
                else
                {
                    keyCodes = enumNames;
                }

                if (ImGui.IsKeyDown(ImGuiKey.Enter) && keyCodes!.Length == 1)
                {
                    value = Enum.Parse(enumType, keyCodes[0]);
                    ClosePopup();
                }

                ImGui.Separator();

                foreach (string keyCode in keyCodes!)
                {
                    if (ImGui.Selectable(keyCode))
                    {
                        value = Enum.Parse(enumType, keyCode);
                        ClosePopup();
                        break;
                    }
                }

                ImGui.EndPopup();
            }
            else
            {
                searchInput = string.Empty;
                popupOpened = false;
            }
        }

        private bool DrawInspectorVariable(string variableName, string variableId, string valueName, out float windowWidth, out Vector2 windowPos)
        {
            ImGui.Text(variableName);
            ImGui.SameLine();

            float remaining = ImGui.GetContentRegionAvail().X;
            float buttonWidth = remaining * 0.8f;
            float buttonHeight = ImGui.GetItemRectSize().Y + 10;

            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + remaining - buttonWidth);
            Vector2 screenCursorPos = ImGui.GetCursorScreenPos();

            windowWidth = buttonWidth < 100 ? 100 : buttonWidth;
            windowPos = screenCursorPos += new Vector2(0, buttonHeight);

            return ImGui.Button($"{valueName}##{variableId}", new Vector2(buttonWidth, buttonHeight));
        }

        private void ClosePopup()
        {
            searchInput = string.Empty;
            cacheDisplayName = null;
            ImGui.CloseCurrentPopup();
        }
    }
}
