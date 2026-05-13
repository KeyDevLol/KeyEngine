namespace KeyEngine.Editor.SupportedTypes
{
    internal class ObjectTypeSupport : TypeSupport
    {
        public override object Render(TypeSupportRenderArgs args)
        {
            object? value = args.Value;

            if (value == null)
            {
                return null!;
            }

            Type type = value.GetType();

            if (SupportedTypes.TryGetTypeSupport(type, out TypeSupport? result))
            {
                value = result.Render(args);
            }

            return value;
        }
    }
}
