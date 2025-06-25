namespace KeyEngine.Editor.SupportedTypes
{
    public static class Supported
    {
        private static readonly Dictionary<Type, TypeSupport> supportedTypesDict = new Dictionary<Type, TypeSupport>()
        {
            { typeof(int), new IntTypeSupport() },
            { typeof(uint), new UIntTypeSupport() },
            { typeof(float), new FloatTypeSupport() },
            { typeof(double), new DoubleTypeSupport() },
            { typeof(Color), new ColorTypeSupport() },
            { typeof(Vector2), new Vector2TypeSupport() },
        };

        public static TypeSupport? GetTypeSupport(Type type)
        {
            if (supportedTypesDict.TryGetValue(type, out TypeSupport? typeSupport))
            {
                return typeSupport;
            }

            return null;
        }

        public static bool TryGetTypeSupport(Type type, out TypeSupport? typeSupport)
        {
            return supportedTypesDict.TryGetValue(type, out typeSupport);
        }
    }
}
