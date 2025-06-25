using KeyEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KeyEngine
{
    public class Entity : Transformable, IComparable<Entity>
    {
        public string Name;
        public bool Active = true;

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

        private readonly List<Component> components;

        public int HashId { get; private set; }

        public Entity(int hashId, string? name)
        {
            Name = name ?? "My name is Edwin";
            HashId = hashId;
            components = new List<Component>();
        }

        static Entity()
        {
           
        }

        public object AddComponent(Type type)
        {
            Component? component = Activator.CreateInstance(type, [this]) as Component;

            Log.Assert(component != null, $"{nameof(Entity)}.{nameof(AddComponent)} failed to add component.", LogType.Error);

            components.Add(component);

            if (SceneManager.SceneIsRunning)
                component.Start();

            return component;
        }

        public T AddComponent<T>() where T : Component
        {
            Component? component = Activator.CreateInstance(typeof(T), [this]) as Component;

            Log.Assert(component != null, $"{nameof(Entity)}.{nameof(AddComponent)} failed to add component.", LogType.Error);

            components.Add(component);

            if (SceneManager.SceneIsRunning)
                component.Start();

            return (T)component;
        }

        public Component AddComponent(Component component)
        {
            components.Add(component);

            if (SceneManager.SceneIsRunning)
                component.Start();

            return component;
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
                component.Deleted();
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is Entity entity)
                return this.HashId == entity.HashId;

            return false;
        }

        public override int GetHashCode()
        {
            return HashId;
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
