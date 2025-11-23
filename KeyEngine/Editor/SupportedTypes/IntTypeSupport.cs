using ImGuiNET;

namespace KeyEngine.Editor.SupportedTypes
{
    public class IntTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            int value = (int)args.Value!;

            ImGui.DragInt(args.VariableName, ref value);

            return value;
        }
    }
}
