using KeyEngine.Rendering;

namespace KeyEngine
{
    public class Entity : Transformable, IComparable<Entity>
    {
        public string? Name { get; set; }
        public bool Active { get; set; } = true;

        public int Layer
        {
            get => _layer;
            set
            {
                if (_layer != value)
                {
                    _layer = value;
                    ECS.RefreshLayer(this);
                }
            }
        }
        private int _layer;

        public readonly Guid Id;

        private readonly List<Component> components;

        protected event Action<Component>? OnComponentAdded;
        protected event Action<Component>? OnComponentRemoved;

        public Entity(string? name = null)
        {
            Name = name ?? "My name is Edwin";
            components = [];
            Id = Guid.NewGuid();
        }

        public object AddComponent(Type type)
        {
            if (Activator.CreateInstance(type, [this]) is not Component component)
                throw new NullReferenceException($"{nameof(component)} is null. Failed to add component.");

            components.Add(component);

            if (SceneManager.SceneIsRunning)
                component.Start();

            OnComponentAdded?.Invoke(component);

            return component;
        }

        public T AddComponent<T>() where T : Component
        {
            return (T)AddComponent(typeof(T));
        }

        public Component AddComponent(Component component)
        {
            ArgumentNullException.ThrowIfNull(component, nameof(component));
            components.Add(component);

            if (SceneManager.SceneIsRunning)
                component.Start();

            OnComponentAdded?.Invoke(component);

            return component;
        }

        public void RemoveComponent(Type type)
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];

                if (component.GetType() == type)
                {
                    components.RemoveAt(i);
                    component.OnDeleted();
                    OnComponentRemoved?.Invoke(component);
                    break;
                }
            }
        }

        public void RemoveComponent<T>() where T : Component
        {
            RemoveComponent(typeof(T));
        }

        public T? GetComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component is T t)
                    return t;
            }

            return null;
        }

        public IEnumerable<Component> GetAllComponents()
        {
            return components;
        }

        internal virtual void CallStart()
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component.Enabled)
                    component.Start();
            }
        }

        internal virtual void CallUpdate(float deltaTime)
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component.Enabled)
                    component.Update(deltaTime);
            }
        }

        internal virtual void CallRender()
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component.Enabled)
                    component.Render();
            }
        }

        internal virtual void CallDeleted()
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                component.OnDeleted();
            }
        }

        internal virtual void CallRenderSelectedGizmos()
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                component.RenderSelectedGizmos();
            }
        }

        internal virtual void CallRenderGizmos()
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                component.RenderGizmos();
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is Entity entity)
                return this.Id == entity.Id;

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(Entity? obj)
        {
            if (obj != null)
            {
                if (Layer > obj.Layer) return -1;
                if (Layer < obj.Layer) return 1;
                return -1;
            }

            return 0;
        }
    }
}
