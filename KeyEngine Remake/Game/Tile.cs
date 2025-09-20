using KeyEngine.Mathematics;
using System.Text.Json.Serialization;

namespace KeyEngine.Game
{
    public struct Tile
    {
        [JsonInclude]
        public SerializableVector2 Position;
        [JsonInclude]
        public int Sprite;
        [JsonIgnore]
        public Entity Entity;

        public Tile(Vector2 position, int sprite, Entity entity)
        {
            Position = new SerializableVector2(position);
            Sprite = sprite;
            Entity = entity;
        }
    }
}
