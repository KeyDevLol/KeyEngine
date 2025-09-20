using ImGuiNET;
using KeyEngine.Mathematics;
using System.Numerics;

namespace KeyEngine.Editor.SupportedTypes
{
    public class ColorTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            Color01 value = ((Color32)args.value!).AsColor01();

            Vector4 vector = new Vector4(value.R, value.G, value.B, value.A);
            ImGui.ColorEdit4(args.name, ref vector, ImGuiColorEditFlags.AlphaPreview);
            return new Color32(vector.X, vector.Y, vector.Z, vector.W);
        }

        private float Get01Color(byte value)
        {
            return (float)(value / 255f);
        }
    }
}