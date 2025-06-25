using System.Reflection;

namespace KeyEngine.Editor.SupportedTypes
{
    public struct TypeSupportRenderArgs
    {
        public string name;
        public object componentInstance;
        public object value;
        public MemberInfo memberInfo;

        public TypeSupportRenderArgs(string name, object componentInstance, object value, MemberInfo memberInfo)
        {
            this.name = name;
            this.componentInstance = componentInstance;
            this.memberInfo = memberInfo;
            this.value = value; 
        }
    }
}