using KeyEngine.Editor.GUI;
using KeyEngine.Serialization;

namespace KeyEngine
{
    public class Component : ISerializable
    {
        public readonly Entity Owner;
        [HideInInspector]
        public bool Enabled { get; set; } = true;

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

        public virtual SerializeData SceneSerialize() => SerializeData.Empty;
        public virtual void SceneDeserialize(SerializeData serializeData) { }

        public void SerializeWrite(ref BinaryWriter writer)
        {
            //throw new NotImplementedException();
        }

        public void SerializeRead(ref BinaryReader reader)
        {
           // throw new NotImplementedException();
        }
    }
}
