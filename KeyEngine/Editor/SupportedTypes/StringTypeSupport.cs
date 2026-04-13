using ImGuiNET;

namespace KeyEngine.Editor.SupportedTypes
{
    public class StringTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            string? value = args.Value as string;
            ImGui.InputText(args.DisplayName, ref value, 1000);

            return value;
        }
    }
}
