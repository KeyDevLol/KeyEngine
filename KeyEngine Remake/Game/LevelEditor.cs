using KeyEngine.Mathematics;
using KeyEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KeyEngine.Game
{
    public class LevelEditor : Component
    {
        [JsonInclude]
        private Dictionary<SerializableVector2, Tile> tiles = [];

        public LevelEditor(Entity owner) : base(owner)
        {
        }

        public override void Update(float deltaTime)
        {
            if (Input.IsAnyMouseButtonPressed)
            {
                Vector2 worldPos = Camera.Main!.ScreenToWorldCoords(Input.MousePosition);
                Vector2 roundedPos = new Vector2(Mathf.Round(worldPos.X), Mathf.Round(worldPos.Y));
                SerializableVector2 roundedVec = new SerializableVector2(roundedPos);

                if (Input.IsMouseButtonDown(MouseButtonCode.Left))
                {
                    if (tiles.ContainsKey(roundedVec) == false)
                    {
                        Entity entity = ECS.AddEntity("Tile");
                        entity.Position = roundedPos;
                        SpriteRenderer sp = entity.AddComponent<SpriteRenderer>();
                        entity.AddComponent<RigidBody>().BodyType = BodyType.Kinematic;

                        tiles.Add(roundedVec, new Tile(roundedPos, SpriteChanger.CurrentSprite, entity));
                        //sp.Texture = TileSprites.Sprites[SpriteChanger.CurrentSprite].Value;
                        
                    }
                }
                else if (Input.IsMouseButtonDown(MouseButtonCode.Right))
                {
                    if (tiles.TryGetValue(roundedVec, out Tile tile))
                    {
                        ECS.RemoveEntity(tile.Entity);
                        tiles.Remove(roundedVec);
                    }
                }
            }

            if (Input.IsKeyDown(KeyCode.F5))
            {
                string json = JsonSerializer.Serialize(tiles.ToArray());

                File.WriteAllText("Assets/Levels/level.json", json);
            }

            if (Input.IsKeyDown(KeyCode.F6))
            {
                string json = File.ReadAllText("Assets/Levels/level.json");
                tiles = JsonSerializer.Deserialize<KeyValuePair<SerializableVector2, Tile>[]>(json).ToDictionary(kv => kv.Key, kv => kv.Value);

                foreach (var l in tiles)
                {
                    Entity entity = ECS.AddEntity("Tile");
                    entity.Position = l.Value.Position.ToVec();
                    SpriteRenderer spriteRenderer = entity.AddComponent<SpriteRenderer>();
                    entity.AddComponent<RigidBody>().BodyType = BodyType.Kinematic;

                    //spriteRenderer.Texture = TileSprites.Sprites[l.Value.Sprite].Value;

                    tiles[l.Key] = new Tile(entity.Position, l.Value.Sprite, entity);
                }
            }
        }
    }
}
