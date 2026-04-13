namespace KeyEngine.Editor.SupportedTypes
{
    public readonly struct TypeSupportRenderArgs
    {
        public readonly string VariableName;
        public readonly string? ClassName;
        public readonly string DisplayName;
        public readonly string EntityId;
        public readonly object? Value;

        public TypeSupportRenderArgs(string displayName, string variableName, string? className, string entityId, object? value)
        {
            DisplayName = displayName;
            VariableName = variableName;
            ClassName = className;
            Value = value;
            EntityId = entityId;
        }
    }
}