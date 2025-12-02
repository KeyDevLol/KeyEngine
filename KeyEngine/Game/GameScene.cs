using KeyEngine.Tests;

namespace KeyEngine.Game
{
    public class GameScene : IScene
    {
        public Entity Player = null!;
        public Player PlayerComponent = null!;

        private LevelEditor LevelEditor = null!; 

        public void Load()
        {
            //Player = ECS.AddEntity("Player");
            //PlayerComponent = Player.AddComponent<Player>();

            Entity ground = ECS.AddEntity("Ground");
            ground.AddComponent<SpriteRenderer>();
            ground.AddComponent<InspectorTestComponent>();

            //SerializationManager.SerializeSceneJson();
            //ground.Position = new Vector2(0, -1);
            //ground.Scale = new Vector2(5, 1);
            //ground.AddComponent<RigidBody>().BodyType = BodyType.Kinematic;

            //Entity levelEditorEntity = ECS.AddEntity("Level Editor");
            //LevelEditor = levelEditorEntity.AddComponent<LevelEditor>();

            //Entity spriteChanger = ECS.AddEntity("Sprite Changer");
            //spriteChanger.AddComponent<SpriteChanger>();

            //Entity instance = ECS.AddEntity("Instance");
            //instance.AddComponent<InstanceRendering>();
        }

        public void Unload()
        {
            
        }
    }
}
