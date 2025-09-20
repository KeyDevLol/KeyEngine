using KeyEngine.Serialization;

namespace KeyEngine
{
    public class Component : ISerializableAsset
    {
        public readonly Entity Owner;
        public bool Enabled = true;

        public Component(Entity owner)
        {
            Owner = owner;
        }

        public virtual void Start() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Render() { }
        public virtual void Deleted() { }
        public virtual object? EngineSerializeRead(ref BinaryReader reader)
        {
            return null;
        }

        public virtual SerializeData SceneSerialize() => SerializeData.Empty;
        public virtual void SceneDeserialize(SerializeData serializeData) { }
    }
}
