using KeyEngine.Editor;
using KeyEngine.Editor.GUI;
using KeyEngine.Editor.Serialization;
using KeyEngine.Graphics;
using KeyEngine.Rendering;
using KeyEngine.Tests;

namespace KeyEngine
{
    //
    public class InspectorTestComponent : Component, IAsset
    {
        static int count;
        private int currentCount;

        public int test = 228;
        public Vector2 vec = Vector2.Down;

        [ShowInInspector]
        public uint uiiint = 228;
        public double doublee = 228;
        public float zalupa = 90.1f;
        public Color color = Color.Blue;
        public Entity? entity = null;

        public bool AssetLoaded => throw new NotImplementedException();

        public InspectorTestComponent(Entity owner) : base(owner)
        {
            currentCount = count;
            count++;
        }

        public override void Start()
        {
            SpriteRenderer spriteRenderer = Owner.AddComponent<SpriteRenderer>();

            //var tex = ;

            spriteRenderer.Texture = AssetsManager.GetAsset<Texture>("Assets/Textures/test.png");
        }

        public override void SceneDeserialize(SerializeData serializeData)
        {
            serializeData.GetData("test", ref test);
            serializeData.GetData("uiiint", ref uiiint);
            serializeData.GetData("doublee", ref doublee);
            serializeData.GetData("zalupa", ref zalupa);
            serializeData.GetData("customSerialiazeTest", ref vec);
            serializeData.GetData("color", ref color);
            serializeData.GetData("null", ref entity);
        }

        public override SerializeData SceneSerialize()
        {
            SerializeData serializeData = new SerializeData();

            serializeData.AddData("test", test);
            serializeData.AddData("uiiint", uiiint);
            serializeData.AddData("doublee", doublee);
            serializeData.AddData("zalupa", zalupa);
            serializeData.AddData("customSerialiazeTest", vec);
            serializeData.AddData("color", color);
            serializeData.AddData("null", entity);
            return serializeData;
        }

        public override void Update(float deltaTime)
        {
            if (currentCount > 0)
                return;

            if (Input.IsKeyDown(KeyCode.Q))
            {
                TestScene.Serialize();
            }

            if (Input.IsKeyDown(KeyCode.W))
            {
                TestScene.Deserialize();
            }

            if (Input.IsKeyDown(KeyCode.E))
            {
                AssetsManager.UnloadAllAssets();
                //Entity entity = new Entity();
                //entity.AddComponent<SpriteRenderer>();

                //ECS.AddEntity(entity);
            }

            if (Input.IsKeyDown(KeyCode.R))
            {
                //ECS.DeleteAllEntities();
                GC.Collect();
            }
        }

        public void LoadAsset(string path)
        {
            throw new NotImplementedException();
        }

        public void UnloadAsset()
        {
            throw new NotImplementedException();
        }
    }
}
