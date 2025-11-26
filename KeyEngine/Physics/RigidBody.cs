using KeyEngine.Editor.GUI;
using KeyEngine.Mathematics;
using KeyEngine.Physics.Extensions;
using KeyEngine.Rendering.Gizmos;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using PBodyType = nkast.Aether.Physics2D.Dynamics.BodyType;
using Vector2 = KeyEngine.Mathematics.Vector2;

namespace KeyEngine.Physics
{
    // TODO: Добавить коллайдеры в виде компонентов
    public partial class RigidBody : Component
    {
        private readonly Body body;
        private Fixture fixture;

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
                    return body.LinearVelocity.AsEngineVector();
                }

                return Vector2.Zero;
            }
            set
            {
                body?.LinearVelocity = value.AsPhysicsVector();
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
            get 
            { 
                return _colliderSize; 
            }
            set 
            { 
                _colliderSize = value;
                RefreshFixtureSizeAndOffset(); 
            }
        }
        private Vector2 _colliderSize = Vector2.One;

        public Vector2 ColliderOffset
        {
            get
            {
                return _colliderOffset;
            }
            set
            {
                _colliderOffset = value;
                RefreshFixtureSizeAndOffset();
            }
        }
        private Vector2 _colliderOffset;

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
                return _friction;
            }
            set
            {
                _friction = Mathf.Clamp(value, 0, float.MaxValue);
                fixture.Friction = _friction;
            }
        }
        private float _friction = 0.4f;

        public float Restitution
        {
            get
            {
                return _restitution;
            }
            set
            {
                _restitution = Mathf.Clamp(value, 0, float.MaxValue);
                fixture.Restitution = _restitution;
            }
        }
        private float _restitution = 1;

        public bool IsSensor
        {
            get
            {
                return _isSensor;
            }
            set
            {
                _isSensor = value;
                fixture.IsSensor = value;
            }
        }
        private bool _isSensor;

        public RigidBody(Entity owner) : base(owner)
        {
            body = new Body();
            body.Position = owner.Position.AsPhysicsVector();
            body.Rotation = owner.Rotation * Mathf.DEG_2_RAD;

            fixture = CreateRectangleFixture(_colliderSize, ColliderOffset, 1);
            body.Add(fixture);

            PhysicsManager.World.Add(body);
            FitSizeWithScale();

            owner.OnTransformChanged += TransformChanged;
        }

        public override void Update(float deltaTime)
        {
            Owner.BeginQuiteMode();
            Owner.Position = body.Position.AsEngineVector();
            Owner.Rotation = Mathf.Repeat(body.Rotation * Mathf.RED_2_DEG, 360);
            Owner.EndQuiteMode();
        }

#if ENABLE_EDITOR
        public override void RenderGizmos()
        {
            GizmosRendering.DrawSquare(Owner.Position + _colliderOffset, _colliderSize, body.Rotation, Color01.Green);
        }
#endif
        public override void OnDeleted()
        {
            Owner.OnTransformChanged -= TransformChanged;
            PhysicsManager.World.Remove(body);
        }

        public void ApplyAngularImpulse(float impulse)
        {
            body?.ApplyAngularImpulse(impulse);
        }

        public void ApplyLinearImpulse(Vector2 impulse)
        {
            body?.ApplyLinearImpulse(impulse.AsPhysicsVector());
        }      
        
        public void ApplyForce(Vector2 force)
        {
            body?.ApplyForce(force.AsPhysicsVector());
        }     
        
        public void ApplyTorque(float torque)
        {
            body?.ApplyTorque(torque);
        }

        public void FitSizeWithScale()
        {
            _colliderSize = Owner.Scale;
            RefreshFixtureSizeAndOffset();
        }

        private void RefreshFixtureSizeAndOffset()
        {
            body.Remove(fixture);
            fixture = CreateRectangleFixture(_colliderSize, _colliderOffset, 1);
            body.Add(fixture);
        }

        private Fixture CreateRectangleFixture(Vector2 size, Vector2 offset, float density)
        {
            Vertices vertices = PolygonTools.CreateRectangle(size.X / 2f, size.Y / 2f);
            vertices.Translate(offset.AsPhysicsVector());
            PolygonShape shape = new PolygonShape(vertices, 1);

            Fixture fixture = new Fixture(shape)
            {
                Friction = _friction,
                Restitution = _restitution,
                IsSensor = _isSensor,
                Tag = this
            };

            return fixture;
        }

        private void TransformChanged()
        {
            body.Position = Owner.Position.AsPhysicsVector();
            body.Rotation = Owner.Rotation * Mathf.DEG_2_RAD;
        }
    }
}
