using ImGuiNET;

namespace KeyEngine.Editor.SupportedTypes
{
    public class BoolTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            bool value = (bool)args.Value!;

            ImGui.Checkbox(args.DisplayName, ref value);

            return value;
        }
    }
}
