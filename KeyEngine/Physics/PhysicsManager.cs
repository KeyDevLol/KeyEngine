using KeyEngine.Mathematics;
using KeyEngine.Physics.Extensions;
using nkast.Aether.Physics2D.Dynamics;

namespace KeyEngine.Physics
{
    public static class PhysicsManager
    {
        public static World World { get; private set; }

        public static Vector2 Gravity
        {
            get => World.Gravity.AsEngineVector();
            set => World.Gravity = value.AsPhysicsVector();
        }

        static PhysicsManager()
        {
            World = new World();
            Gravity = new Vector2(0, -9.82f);
        }

        public static void Update(float deltaTime)
        {
            if (World.BodyList.Count > 0)
                World.Step(Mathf.Clamp(deltaTime, 0, 0.1f));
        }
    }
}
