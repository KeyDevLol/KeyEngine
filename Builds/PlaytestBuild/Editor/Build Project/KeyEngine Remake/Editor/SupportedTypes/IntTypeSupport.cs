using ImGuiNET;

namespace KeyEngine.Editor.SupportedTypes
{
    public class IntTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            int value = (int)args.value;

            ImGui.DragInt(args.name, ref value);

            return value;
        }
    }
}
