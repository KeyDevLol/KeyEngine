using ImGuiNET;
using System.Numerics;

namespace KeyEngine.Editor.SupportedTypes
{
    public class ColorTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            Color value = (Color)args.value!;

            Vector4 vector = new Vector4(Color.ToFloat(value.R), Color.ToFloat(value.G),
                Color.ToFloat(value.B), Color.ToFloat(value.A));
            ImGui.ColorEdit4(args.name, ref vector, ImGuiColorEditFlags.AlphaPreview);
            return new Color(vector.X, vector.Y, vector.Z, vector.W);
        }

        private float Get01Color(byte value)
        {
            return (float)(value / 255f);
        }
    }
}