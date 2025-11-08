using KeyEngine.Mathematics;
using KeyEngine.Rendering;

namespace KeyEngine.Samples
{
    public class PhysicsScene : IScene
    {
        public void Load()
        {
            Camera.Main!.Zoom = 40;
            PhysicsManager.ContinuousPhysicsEnabled = false;

            Entity spawner = ECS.AddEntity("Square Spawner");
            spawner.AddComponent<SquareSpawner>();

            Entity lWall = ECS.AddEntity("Left Wall");
            Entity rWall = ECS.AddEntity("Right Wall");


            lWall.Scale = new Vector2(0.5f, 20f);
            rWall.Scale = new Vector2(0.5f, 20f);
            lWall.Position = new Vector2(14.750f, 0);
            rWall.Position = new Vector2(-14.750f, 0);

            lWall.AddComponent<RigidBody>().BodyType = BodyType.Kinematic;
            rWall.AddComponent<RigidBody>().BodyType = BodyType.Kinematic;
            lWall.AddComponent<SpriteRenderer>();
            rWall.AddComponent<SpriteRenderer>();

            Entity ground = ECS.AddEntity("Ground");

            ground.Scale = new Vector2(30, 0.5f);
            ground.Position = new Vector2(0, -10);

            ground.AddComponent<RigidBody>().BodyType = BodyType.Kinematic;
            ground.AddComponent<SpriteRenderer>();
        }

        public void Unload() { }

        private class SquareSpawner(Entity owner) : Component(owner)
        {
            public bool ContinuousPhysicsEnabled 
            {
                get => PhysicsManager.ContinuousPhysicsEnabled;
                set => PhysicsManager.ContinuousPhysicsEnabled = value;
            }

            private readonly Random random = new Random();
            private int colorCounter;

            public override void Update(float deltaTime)
            {
                if (!Input.IsKeyDown(KeyCode.Space))
                    return;

                Entity entity = ECS.AddEntity();
                entity.Position = new Vector2(random.Next(0, 4), 10);
                entity.Scale = new Vector2(0.25f, 0.25f);

                RigidBody rb = entity.AddComponent<RigidBody>();
                rb.BodyType = BodyType.Dynamic;
                rb.SleepingAllowed = false;

                SpriteRenderer sp = entity.AddComponent<SpriteRenderer>();

                colorCounter = (int)Mathf.Repeat(colorCounter, 4);

                switch (colorCounter)
                {
                    case 0:
                        sp.Color = Color32.Blue;
                        break;                    
                    case 1:
                        sp.Color = Color32.Green;
                        break;                    
                    case 2:
                        sp.Color = Color32.Red;
                        break;                    
                    case 3:
                        sp.Color = Color32.Yellow;
                        break;
                }

                colorCounter++;
            }
        }
    }
}
