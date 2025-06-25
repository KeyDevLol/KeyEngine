using System.Reflection;

namespace KeyEngine.Editor.GUI
{
    public struct CachedComponent
    {
        public MemberInfo[] variables;
        public Type componentType;
        public Component component;

        public CachedComponent(Component component)
        {
            variables = Utils.SelectOnlyAttributes(Utils.GetAllObjectVariables(component)).ToArray();
            componentType = component.GetType();
            this.component = component;
        }
    }
}
