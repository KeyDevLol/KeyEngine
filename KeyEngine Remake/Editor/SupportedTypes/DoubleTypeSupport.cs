using ImGuiNET;

namespace KeyEngine.Editor.SupportedTypes
{
    public class DoubleTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            double value = (double)args.value!;

            ImGui.InputDouble(args.name, ref value, 0.1f);

            return (double)value;
        }
    }
}
