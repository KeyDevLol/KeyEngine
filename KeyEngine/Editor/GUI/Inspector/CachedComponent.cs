using System.Reflection;

namespace KeyEngine.Editor.GUI
{
    public readonly struct CachedComponent
    {
        public readonly IEnumerable<VariableInfo> Variables;
        public readonly Type ComponentType;
        public readonly Component Component;

        public CachedComponent(Component component)
        {
            var members = Utils.SelectOnlyVisible(Utils.GetAllObjectVariables(component)).ToArray();
            Variables = GetVariables(members);
            ComponentType = component.GetType();
            Component = component;
        }

        private IEnumerable<VariableInfo> GetVariables(IEnumerable<MemberInfo> members)
        {
            foreach (var member in members)
            {
                yield return new VariableInfo(member);
            }
        }
    }
}
