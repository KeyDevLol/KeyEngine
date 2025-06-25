using ImGuiNET;

namespace KeyEngine.Editor.SupportedTypes
{
    public class UIntTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            uint value = (uint)args.value!;
            float floatValue = value;

            ImGui.DragFloat(args.name, ref floatValue, 1, 0);

            return (uint)floatValue;
        }
    }
}
