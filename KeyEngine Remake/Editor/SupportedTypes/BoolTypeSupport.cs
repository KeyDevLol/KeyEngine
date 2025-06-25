using ImGuiNET;

namespace KeyEngine.Editor.SupportedTypes
{
    public class BoolTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            bool value = (bool)args.value!;

            ImGui.Checkbox(args.name, ref value);

            return value;
        }
    }
}
