using ImGuiNET;

namespace KeyEngine.Editor.SupportedTypes
{
    public class Vector2TypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            Vector2 value = (Vector2)args.value;
            System.Numerics.Vector2 vector = value;

            ImGui.DragFloat2(args.name, ref vector, 0.1f);

            return (Vector2)vector;
        }
    }
}
