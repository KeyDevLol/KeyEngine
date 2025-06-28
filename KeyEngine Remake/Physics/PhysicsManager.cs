using Genbox.VelcroPhysics.Dynamics;
using PVector2 = Microsoft.Xna.Framework.Vector2;

namespace KeyEngine
{
    public static class PhysicsManager
    {
        public static World World { get; private set; } = new World(new PVector2(0, -9.82f));

        public static Vector2 Gravity
        {
            get { return _gravity; }
            set { _gravity = value; World.Gravity = _gravity; }
        }
        private static Vector2 _gravity = new Vector2(0, -9.82f);

        public static bool ContinuousPhysicsEnabled
        {
            get => World.ContinuousPhysicsEnabled;
            set => World.ContinuousPhysicsEnabled = value;
        }

        public static void Update(float deltaTime)
        {
            World.EnableDiagnostics = false;

            if (World.BodyList.Count > 0)
                World.Step(deltaTime);

            World.IsLocked = false;
        }

        public static bool RayCast(Vector2 point1, Vector2 point2, out RigidBody? rb)
        {
            List<Fixture> fixtures = World.RayCast(point1, point2);

            if (fixtures.Count > 0)
            {
                rb = (RigidBody)fixtures[0].UserData;
                return true;
            }

            rb = null;
            return false;
        }
    }
}
