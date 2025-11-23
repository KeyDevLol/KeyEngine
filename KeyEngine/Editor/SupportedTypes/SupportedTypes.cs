using KeyEngine.Mathematics;
using System.Diagnostics.CodeAnalysis;

namespace KeyEngine.Editor.SupportedTypes
{
    public static class SupportedTypes
    {
        private static readonly Dictionary<Type, TypeSupport> supportedTypesDict = new Dictionary<Type, TypeSupport>()
        {
            { typeof(bool), new BoolTypeSupport() },
            { typeof(int), new IntTypeSupport() },
            { typeof(uint), new UIntTypeSupport() },
            { typeof(float), new FloatTypeSupport() },
            { typeof(double), new DoubleTypeSupport() },
            { typeof(Color32), new ColorTypeSupport() },
            { typeof(Vector2), new Vector2TypeSupport() },
            { typeof(object), new ObjectTypeSupport() },
            { typeof(KeyCode), new KeyCodeTypeSupport() },
        };

        public static TypeSupport? GetTypeSupport(Type type)
        {
            if (supportedTypesDict.TryGetValue(type, out TypeSupport? typeSupport))
            {
                return typeSupport;
            }

            return null;
        }

        
        public static bool TryGetTypeSupport(Type type, [MaybeNullWhen(false)] out TypeSupport typeSupport)
        {
            return supportedTypesDict.TryGetValue(type, out typeSupport);
        }
    }
}
