using ImGuiNET;

namespace KeyEngine.Editor.SupportedTypes
{
    public class FloatTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            float value = (float)args.value!;

            ImGui.DragFloat(args.name, ref value, 0.1f);

            return value;
        }
    }
}
