
using KeyEngine.Mathematics;

namespace KeyEngine.Game
{
    public class Player : Component
    {
        private RigidBody rb = null!;
        public float Speed = 550;
        public float CameraSpeed = 7;
        public float JumpForce = 7;

        public Player(Entity owner) : base(owner) { }

        public override void Start()
        {
            Owner.AddComponent<SpriteRenderer>();
            rb = Owner.AddComponent<RigidBody>();
            rb.BodyType = BodyType.Dynamic;
            rb.FreezeRotation = true;
            rb.SleepingAllowed = false;
            rb.ColliderSize = new Vector2(0.99f, 0.99f);

            rb.Friction = 0;
        }

        public override void Update(float deltaTime)
        {
            float x = Input.GetAxisRaw(KeyCode.A, KeyCode.D);

            if (PhysicsManager.RayCast(Owner.Position - new Vector2(0, 1), Owner.Position - new Vector2(0, 2), out RigidBody? other))
            {
                Log.Print(other.Owner.Name);
            }

            if (Input.IsKeyPressed(KeyCode.Space))
            {
                rb.LinearVelocity += new Vector2(0, JumpForce);
            }

            rb.LinearVelocity = new Vector2(x * Speed * deltaTime, rb.LinearVelocity.Y);
        }
    }
}
