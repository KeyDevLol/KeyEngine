using System.Reflection;

namespace KeyEngine.Editor.SupportedTypes
{
    public struct TypeSupportRenderArgs
    {
        public readonly MemberInfo MemberInfo;
        public readonly string VariableName => MemberInfo.Name;
        public readonly string DisplayName;
        public readonly string EntityId;
        public readonly string? ClassName => MemberInfo.ReflectedType?.Name;
        public readonly object? Value;
        public readonly object ComponentInstance;

        public TypeSupportRenderArgs(string displayName, string entityId, object componentInstance, object? value, MemberInfo memberInfo)
        {
            DisplayName = displayName;
            ComponentInstance = componentInstance;
            MemberInfo = memberInfo;
            Value = value;
            EntityId = entityId;
        }
    }
}