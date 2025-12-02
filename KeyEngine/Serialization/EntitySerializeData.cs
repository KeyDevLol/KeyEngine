using System.Numerics;

namespace KeyEngine.Serialization
{
    public struct EntitySerializeData
    {
        public string? Name;
        public Vector2 Position;
        public Vector2 Scale;
        public float Rotation;

        public int Layer;
        public bool Active;

        public List<ComponentSerializeData> Components = [];

        public EntitySerializeData() { }
    }
}
