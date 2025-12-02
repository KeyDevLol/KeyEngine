using KeyEngine.Editor.Attributes;
using KeyEngine.Serialization;

namespace KeyEngine
{
    public class Component : ISerializable
    {
        public readonly Entity Owner;
        [HideInInspector] public bool Enabled { get; set; } = true;

        public Component(Entity owner)
        {
            Owner = owner;
        }

        public virtual void Start() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Render() { }
        public virtual void OnDeleted() { }
        public virtual void OnDisabled() { }
        public virtual void OnEnabled() { }
        public virtual void RenderSelectedGizmos() { }
        public virtual void RenderGizmos() { }

        public virtual SerializeData EditorSerialize() => new SerializeData();
        public virtual void EditorDeserialize(SerializeData data) { }
    }
}
