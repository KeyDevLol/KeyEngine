using Genbox.VelcroPhysics.Collision.ContactSystem;
using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Definitions;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using KeyEngine.Editor.GUI;
using PBodyType = Genbox.VelcroPhysics.Dynamics.BodyType;

namespace KeyEngine
{
    public partial class RigidBody : Component
    {
        private readonly Body body;
        private Shape shape;

        private Vector2 lastScale;
        private CollisionArgs callCollisionEnter = new CollisionArgs();
        private CollisionArgs callCollisionExit = new CollisionArgs();

        public Action<RigidBody>? CollisionEnter;
        public Action<RigidBody>? CollisionExit;

        public BodyType BodyType
        {
            get { return _bodyType; }
            set { _bodyType = value; body.BodyType = (PBodyType)_bodyType; }
        }
        private BodyType _bodyType;

        [HideInInspector]
        public Vector2 LinearVelocity
        {
            get
            {
                if (body != null)
                {
                    return body.LinearVelocity;
                }

                return Vector2.Zero;
            }
            set
            {
                if (body != null)
                {
                    body.LinearVelocity = value;
                }
            }
        }

        [HideInInspector]
        public float AngularVelocity
        {
            get
            {
                if (body != null)
                {
                    return body.AngularVelocity * Mathf.RED_2_DEG;
                }

                return 0;
            }
            set
            {
                if (body != null)
                {
                    body.AngularVelocity = value * Mathf.DEG_2_RAD;
                }
            }
        }

        public Vector2 ColliderSize
        {
            get { return _colliderSize; }
            set { _colliderSize = value; ChangeShapeSize(_colliderSize.X, _colliderSize.Y); }
        }
        private Vector2 _colliderSize = Vector2.One;

        public Vector2 ColliderOffset;

        public bool IsTrigger
        {
            get => body.FixtureList[0].IsSensor;
            set => body.FixtureList[0].IsSensor = value;
        }

        public float GravityScale
        {
            get => body.GravityScale;
            set => body.GravityScale = value;
        }

        public float Mass
        {
            get => body.Mass;
            set => body.Mass = value;
        }

        public bool SleepingAllowed
        {
            get => body.SleepingAllowed;
            set => body.SleepingAllowed = value;
        }

        public bool FreezeRotation
        {
            get => body.FixedRotation;
            set => body.FixedRotation = value;
        }

        public float Friction
        { 
            get 
            {
                return body.FixtureList[0].Friction;
            }
            set
            {
                body.FixtureList[0].Friction = Mathf.Clamp(value, 0, float.MaxValue);
            }
        }

        public float Restitution
        {
            get
            {
                return body.FixtureList[0].Restitution;
            }
            set
            {
                body.FixtureList[0].Restitution = Mathf.Clamp(value, 0, float.MaxValue);
            }
        }

        public RigidBody(Entity owner) : base(owner)
        {
            BodyDef bodyDef = new BodyDef
            {
                Position = owner.Position,
                Angle = owner.Rotation * Mathf.DEG_2_RAD,
                Type = (PBodyType)BodyType,
                UserData = this
            };
            body = BodyFactory.CreateFromDef(PhysicsManager.World, bodyDef);

            shape = new PolygonShape(PolygonUtils.CreateRectangle(owner.Scale.X / 2, owner.Scale.Y / 2), 1);

            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.Shape = shape;
            fixtureDef.Friction = 0.5f;
            fixtureDef.UserData = this;
            body.AddFixture(fixtureDef);

            owner.OnTransformChanged += TransformChanged;

            lastScale = owner.Scale;
            _colliderSize = owner.Scale;

            body.OnCollision += OnCollisionEnter;
            body.OnSeparation += OnCollisionExit;
            body.Mass = 1;
        }

        private void OnCollisionEnter(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            callCollisionEnter.Call = true;

            callCollisionEnter.FixtureA = fixtureA;
            callCollisionEnter.FixtureB = fixtureB;
            callCollisionEnter.Contact = contact;
        }

        private void OnCollisionExit(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            callCollisionExit.Call = true;

            callCollisionExit.FixtureA = fixtureA;
            callCollisionExit.FixtureB = fixtureB;
            callCollisionExit.Contact = contact;
        }

        private void TransformChanged()
        {
            body.SetTransform(Owner.Position, Owner.Rotation);

            if (lastScale != Owner.Scale)
            {
                ChangeShapeSize(Owner.Scale.X, Owner.Scale.Y);
                lastScale = Owner.Scale;
            }
        }

        public override void Update(float deltaTime)
        {
            Owner.BeginQuiteMode();
            Owner.Position = body.Position;
            Owner.Rotation = body.Rotation * Mathf.Repeat(body.Rotation * Mathf.RED_2_DEG, 360);
            Owner.EndQuiteMode();

            //if (PhysicsManager.World.IsLocked == false)
            //{
            //    if (callCollisionEnter.Call == true)
            //    {
            //        onCollisionEnter?.Invoke((RigidBody)callCollisionEnter.FixtureB.UserData);
            //        callCollisionEnter.Call = false;
            //    }
                
            //    if (callCollisionExit.Call == true)
            //    {
            //        onCollisionExit?.Invoke((RigidBody)callCollisionExit.FixtureB.UserData);
            //        callCollisionExit.Call = false;
            //    }
            //}
        }

        public override void Deleted()
        {
            Owner.OnTransformChanged -= TransformChanged;
            PhysicsManager.World.RemoveBody(body);
        }

        public void ApplyAngularImpulse(float impulse)
        {
            body?.ApplyAngularImpulse(impulse);
        }

        public void ApplyLinearImpulse(Vector2 impulse)
        {
            body?.ApplyLinearImpulse(impulse);
        }      
        
        public void ApplyForce(Vector2 force)
        {
            body?.ApplyForce(force);
        }     
        
        public void ApplyTorque(float torque)
        {
            body?.ApplyTorque(torque);
        }

        private void ChangeShapeSize(float x, float y)
        {
            shape = new PolygonShape(1);

            ((PolygonShape)shape).SetAsBox(Mathf.Clamp(x / 2, 0.01f, float.MaxValue),
                Mathf.Clamp(y / 2, 0.01f, float.MaxValue));

            float mass = body.Mass;

            FixtureDef fixtureDef = new FixtureDef
            {
                Shape = shape,
                Friction = Friction,
                Restitution = Restitution,
                IsSensor = IsTrigger,
                UserData = this,
            };

            body.RemoveFixture(body.FixtureList[0]);
            body.AddFixture(fixtureDef);

            body.Mass = mass;
        }

        private struct CollisionArgs
        {
            public bool Call;
            public Fixture FixtureA;
            public Fixture FixtureB;
            public Contact Contact;
        }
    }
}
