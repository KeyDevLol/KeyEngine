using ImGuiNET;
using System.Numerics;

namespace KeyEngine.Editor.SupportedTypes
{
    public class KeyCodeTypeSupport : TypeSupport
    {
        private static string[]? keyCodeValues;
        private string? searchInput = string.Empty;
        private bool popupOpened;
        private float windowHeight = 300;

        public override object Render(TypeSupportRenderArgs args)
        {
            keyCodeValues ??= Enum.GetNames<KeyCode>();

            KeyCode value = (KeyCode)args.Value!;
            string valueName = value == 0 ? KeyCode.Unknown.ToString() : value.ToString();

            if (DrawInspectorVariable(args.VariableName, valueName, out float windowWidth, out Vector2 windowPos))
            {
                if (popupOpened)
                {
                    ClosePopup();
                }
                else
                {
                    popupOpened = true;
                    ImGui.OpenPopup(args.DisplayName);
                }
            }

            if (popupOpened)
            {
                ImGuiViewportPtr viewport = ImGui.GetMainViewport();

                ImGui.SetNextWindowSizeConstraints(Vector2.Zero, new Vector2(windowWidth, viewport.WorkSize.Y - windowPos.Y));
                ImGui.SetNextWindowPos(windowPos);

                DrawPopup(args, ref value);
            }

            return value;
        }

        private void DrawPopup(TypeSupportRenderArgs args, ref KeyCode value)
        {
            if (ImGui.BeginPopup(args.DisplayName, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.Popup))
            {
                ImGui.InputTextWithHint("##Search", "Search...", ref searchInput, 60, ImGuiInputTextFlags.NoHorizontalScroll);

                string[]? keyCodes = null;

                if (!string.IsNullOrEmpty(searchInput))
                {
                    StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
                    string lowerSearchInput = searchInput.ToLower();
                    keyCodes = keyCodeValues!.Where(s => s.StartsWith(lowerSearchInput, comparison) || s.Equals(lowerSearchInput, comparison)).ToArray();
                }
                else
                {
                    keyCodes = keyCodeValues;
                }

                if (ImGui.IsKeyDown(ImGuiKey.Enter) && keyCodes!.Length == 1)
                {
                    value = Enum.Parse<KeyCode>(keyCodes[0]);
                    ClosePopup();
                }

                ImGui.Separator();

                foreach (string keyCode in keyCodes!)
                {
                    if (ImGui.Selectable(keyCode))
                    {
                        value = Enum.Parse<KeyCode>(keyCode);
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

        private bool DrawInspectorVariable(string variableText, string valueName, out float windowWidth, out Vector2 windowPos)
        {
            ImGui.Text(variableText);
            ImGui.SameLine();

            float remaining = ImGui.GetContentRegionAvail().X;
            float buttonWidth = remaining * 0.8f;
            float buttonHeight = ImGui.GetItemRectSize().Y + 10;

            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + remaining - buttonWidth);
            Vector2 screenCursorPos = ImGui.GetCursorScreenPos();

            windowWidth = buttonWidth < 100 ? 100 : buttonWidth;
            windowPos = screenCursorPos += new Vector2(0, buttonHeight);

            return ImGui.Button(valueName, new Vector2(buttonWidth, buttonHeight));
        }

        private void ClosePopup()
        {
            searchInput = string.Empty;
            popupOpened = false;
            ImGui.CloseCurrentPopup();
        }
    }
}
