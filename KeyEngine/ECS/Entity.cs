using KeyEngine.Rendering;

namespace KeyEngine
{
    /// <summary>
    /// Represents a game entity with transform and components
    /// </summary>
    public class Entity : Transformable, IComparable<Entity>
    {
        /// <summary>
        /// Gets or sets the entity's name.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating if the entity is active.
        /// </summary>
        public bool Active { get; set; } = true;
        /// <summary>
        /// !Needs documentation
        /// </summary>
        public bool IsAlive { get; private set; } = true;
        /// <summary>
        /// If true, the entity will not be destroyed when loading a new scene.
        /// </summary>
        public bool SceneImmunity { get; set; }

        /// <summary>
        /// Gets or sets the rendering order.
        /// </summary>
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

        /// <summary>
        /// Unique identifier for tracking and referencing this entity instance.
        /// </summary>
        public readonly Guid Id;

        private readonly List<Component> components;

        protected event Action<Component>? OnComponentAdded;
        protected event Action<Component>? OnComponentRemoved;

        internal Entity(string? name = null)
        {
            Name = name ?? "New Entity";
            components = [];
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Adds a new component of the given type to the entity.
        /// </summary>
        /// <returns>
        /// The newly created component instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when type is null.</exception>
        public object AddComponent(Type type)
        {
            if (Activator.CreateInstance(type, [this]) is not Component component)
                throw new NullReferenceException($"{nameof(component)} is null. Failed to add component.");

            components.Add(component);

            if (SceneManager.IsSceneRunning)
                component.Start();

            OnComponentAdded?.Invoke(component);

            return component;
        }

        /// <summary>
        /// Adds a new component of the given type to the entity.
        /// </summary>
        /// <returns>
        /// The newly created component instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when type is null.</exception>
        public T AddComponent<T>() where T : Component
        {
            return (T)AddComponent(typeof(T));
        }

        /// <summary>
        /// Adds a new component of the given type to the entity.
        /// </summary>
        /// <returns>
        /// The newly created component instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when component is null.</exception>
        public Component AddComponent(Component component)
        {
            ArgumentNullException.ThrowIfNull(component, nameof(component));
            components.Add(component);

            if (SceneManager.IsSceneRunning)
                component.Start();

            OnComponentAdded?.Invoke(component);

            return component;
        }

        /// <summary>
        /// Finds and removes the first component matching the given type.
        /// </summary>
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

        /// <summary>
        /// Finds and removes the first component matching the given type.
        /// </summary>
        public void RemoveComponent<T>() where T : Component
        {
            RemoveComponent(typeof(T));
        }

        /// <summary>
        /// Retrieves a component of the specified generic type from the entity.
        /// </summary>
        /// <typeparam name="T">Component type to search for.</typeparam>
        /// <returns>
        /// The component instance if found; otherwise <see langword="null"/>.
        /// </returns>
        /// <remarks>
        /// Returns the first matching component.
        /// </remarks>
        public T? GetComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component is T t)
                    return t;
            }

            return null;
        }

        /// <summary>
        /// Returns all components attached to this entity.
        /// </summary>
        public IEnumerable<Component> GetAllComponents()
        {
            return components;
        }

        /// <summary>
        /// !Needs documentation
        /// </summary>
        public void Destroy()
        {
            IsAlive = false;
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
