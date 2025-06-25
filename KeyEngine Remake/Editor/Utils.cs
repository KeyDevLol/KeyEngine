using System.Reflection;

namespace KeyEngine.Editor
{
    public static class Utils
    {
        public static IEnumerable<MemberInfo> SelectOnlyVisible(IEnumerable<MemberInfo> values)
        {
            foreach (MemberInfo info in values)
            {
                if (ShouldBeRendered(info))
                {
                    yield return info;
                }
            }
        }

        public static IEnumerable<MemberInfo> GetAllObjectVariables(object component)
        {
            IEnumerable<MemberInfo> fields = GetObjectFields(component);
            IEnumerable<MemberInfo> properties = GetObjectProperties(component);

            return fields.Concat(properties);
        }

        public static IEnumerable<MemberInfo> GetObjectFields(object component)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                        BindingFlags.Instance | BindingFlags.SetField;
            Type type = component.GetType();

            return type.GetFields(bindingFlags).Cast<MemberInfo>();
        }

        public static IEnumerable<MemberInfo> GetObjectProperties(object component)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.SetField;
            Type type = component.GetType();

            return type.GetProperties(bindingFlags).Cast<MemberInfo>();
        }

        public static bool ShouldBeRendered(MemberInfo variable)
        {
            if (variable is FieldInfo field)
            {
                return field.IsPublic && !field.IsInitOnly;
            }
            else if (variable is PropertyInfo property)
            {
                MethodInfo? getMethod = property.GetGetMethod(true);

                return getMethod != null && getMethod.IsPublic;
            }

            return false;
        }
    }
}
