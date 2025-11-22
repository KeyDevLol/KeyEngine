using System.Reflection;
using System.Linq;

namespace KeyEngine.Editor.SupportedTypes
{
    public abstract class TypeSupport
    {
        public abstract object Render(TypeSupportRenderArgs args);

        public virtual string GetVariableName(MemberInfo memberInfo)
        {
            return $"{memberInfo.Name}##{memberInfo.ReflectedType?.Name}";
        }
    }
}
