using System.Reflection;

namespace KeyEngine.Editor
{
    public static class MemberInfoExtensions
    {
        public static void SetValue(this MemberInfo member, object property, object value)
        {
            if (member.MemberType == MemberTypes.Property)
                ((PropertyInfo)member).SetValue(property, value, null);
            else if (member.MemberType == MemberTypes.Field)
                ((FieldInfo)member).SetValue(property, value);
            else
                throw new Exception("Property must be of type FieldInfo or PropertyInfo");
        }

        public static object GetValue(this MemberInfo member, object property)
        {
            if (member.MemberType == MemberTypes.Property)
                return ((PropertyInfo)member).GetValue(property, null);
            else if (member.MemberType == MemberTypes.Field)
                return ((FieldInfo)member).GetValue(property);
            else
                throw new Exception("Property must be of type FieldInfo or PropertyInfo");
        }

        public static bool IsPublic(this MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Property)
            {
                MethodInfo getSetMethod = ((PropertyInfo)member).GetSetMethod(true);

                return getSetMethod != null;
            }
            else if (member.MemberType == MemberTypes.Field)
                return ((FieldInfo)member).IsPublic;
            else
                throw new Exception("Property must be of type FieldInfo or PropertyInfo");
        }

        public static Type GetVariableType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
            }
        }

        //public static void K<T>(this HashSet<T> hs, T item)
        //{
        //    Type type = hs.GetType();

        //    int[] m_buckets = (int[])type.GetType().GetField("m_buckets", BindingFlags.NonPublic).GetValue(hs);
            

        //    if (m_buckets != null)
        //    {
        //        int hashCode = (int)type.GetMethod("InternalGetHashCode", BindingFlags.NonPublic).Invoke(hs, [item]);
        //        // see note at "HashSet" level describing why "- 1" appears in for loop
        //        for (int i = m_buckets[hashCode % m_buckets.Length] - 1; i >= 0; i = m_slots[i].next)
        //        {
        //            if (m_slots[i].hashCode == hashCode && m_comparer.Equals(m_slots[i].value, item))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //}
    }
}