namespace KeyEngine.Editor.SupportedTypes
{
    public sealed class TypeSupportRenderArgs
    {
        public readonly string VariableName;
        public readonly string VariableId;
        public readonly string DisplayName;
        public readonly string? ClassName;
        public readonly string EntityId;
        public readonly object? Value;

        public TypeSupportRenderArgs(string variableName, string variableId, string? className, string entityId, object? value)
        {
            VariableId = variableId ?? throw new ArgumentNullException(nameof(variableId));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId));
            ClassName = className;
            Value = value;

            DisplayName = $"{VariableName}##{variableId}";
        }
    }
}