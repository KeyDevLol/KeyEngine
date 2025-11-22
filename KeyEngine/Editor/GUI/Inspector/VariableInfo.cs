using System.Reflection;

namespace KeyEngine.Editor.GUI
{
    public class VariableInfo
    {
        public readonly MemberInfo MemberInfo;
        public readonly Type Type;
        public readonly Type? ReflectedType;
        public readonly MemberTypes MemberType;
        public readonly bool IsReadonly;

        private readonly FieldInfo? fieldInfo;
        private readonly PropertyInfo? propertyInfo;

        public VariableInfo(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
            MemberType = memberInfo.MemberType;
            Type = memberInfo.GetVariableType();
            ReflectedType = memberInfo.ReflectedType;

            if (MemberType == MemberTypes.Field)
                fieldInfo = (FieldInfo)memberInfo;
            else if (MemberType == MemberTypes.Property)
                propertyInfo = (PropertyInfo)memberInfo;
            else
                throw new Exception("MemberInfo must be if type FieldInfo or PropertyInfo");

            if (memberInfo is PropertyInfo p)
            {
                MethodInfo? setMethod = p.GetSetMethod();

                if (setMethod == null || !setMethod.IsPublic)
                {
                    IsReadonly = true;
                }
            }
        }

        public object? GetValue(object? instance)
        {
            if (fieldInfo != null)
                return fieldInfo.GetValue(instance);
            else if (propertyInfo != null)
                return propertyInfo.GetValue(instance);
            else
                throw new Exception("Failed to get value. FieldInfo and PropertyInfo is null");
        }

        public void SetValue(object? instance, object? value)
        {
            if (fieldInfo != null)
                fieldInfo.SetValue(instance, value);
            else if (propertyInfo != null)
                propertyInfo.SetValue(instance, value);
            else
                throw new Exception("Failed to set value. FieldInfo and PropertyInfo is null");
        }
    }
}