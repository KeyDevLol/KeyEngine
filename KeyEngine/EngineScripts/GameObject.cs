using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace KeyEngine
{
    public class GameObject : IGraphic, IHaveComponents
    {
        public RectangleShape shape;
        public List<Component> components;

        public bool active = true;
        public int layer;

        private int objectIndex;

        public GameObject()
        {
            shape = new RectangleShape();
            components = new List<Component>();

            objectIndex = MainClass.renderObjects.Count;

            MainClass.renderObjects.Add(this);
        }

        ~GameObject()
        {
            Console.WriteLine("Какой-то обжект был удален.");
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return (T)component;
                }
            }

            return null;
        }

        public bool TryGetComponent<T>(out T obj) where T : Component
        {
            foreach (Component component in components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    obj = (T)component;
                    return true;
                }
            }

            obj = null;

            return false;
        }

        public bool ComponentExists<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return true;
                }
            }

            return false;
        }

        public T GetComponentInterface<T>() where T : class
        {
            if (typeof(T).IsInterface)
            {
                foreach (Component component in components)
                {
                    Type[] classInterfaces = component.GetType().GetInterfaces();

                    for (int i = 0; i < classInterfaces.Length; i++)
                    {
                        if (classInterfaces[i].Name == typeof(T).Name)
                        {
                            return component as T;
                        }
                    }
                }
            }

            return null;
        }

        public void AddComponent(Component component)
        {
            component.gameObject = this;

            components.Add(component);

            MainClass.Components.Add(component);

            //component.Start();
        }

        public void AddComponent<T>() where T : Component
        {
            object instance = Activator.CreateInstance(typeof(T));
            Component component = instance as Component;

            component.gameObject = this;

            components.Add(component);

            MainClass.Components.Add(component);

            //component.Start();
        }

        public void AddComponent<T>(params object[] componentParams) where T : Component
        {
            object instance = Activator.CreateInstance(typeof(T), args:componentParams);
            Component component = instance as Component;

            component.gameObject = this;

            components.Add(component);

            MainClass.Components.Add(component);

            //component.Start();
        }

        public void RemoveComponent<T>() where T : Component
        {
            T component = GetComponent<T>();

            component.OnObjectDestroy();
            MainClass.Components.Remove(component);
        }

        /// <summary>
        /// Изменяет порядок рендера объекта
        /// </summary>
        /// <param name="index">Рендер индекс MainClass.RenderObjects</param>
        public void ChangeRenderOrder(int index)
        {
            if (index > MainClass.renderObjects.Count - 1)
            {
                MainClass.renderObjects.Remove(this);
                MainClass.renderObjects.Add(this);

                return;
            }
        }

        public static void Destroy(ref GameObject obj)
        {
            int count = obj.components.Count;

            for (int i = 0; i < count; i++)
            {
                obj.components[i].OnObjectDestroy();
                //obj.components[i].gameObject = null;

                MainClass.Components.Remove(obj.components[i]);
            }

            obj.components.Clear();
            obj.components = null;

            MainClass.renderObjects.Remove(obj);

            obj.shape.Dispose();

            obj = null;
        }

        public static T FindComponentByType<T>() where T : Component
        {
            for (int i = 0; i <= MainClass.renderObjects.Count; i++)
            {
                var currObj = MainClass.renderObjects[i] as GameObject;

                if (currObj.TryGetComponent(out T component))
                {
                    return component;
                }
            }

            return null;
        }

        public static T[] FindComponentsByType<T>() where T : Component
        {
            List<T> finded = new List<T>();

            for (int i = 0; i <= MainClass.renderObjects.Count; i++)
            {
                var currObj = MainClass.renderObjects[i] as GameObject;

                if (currObj.TryGetComponent(out T component))
                {
                    finded.Add(component);
                }
            }

            if (finded.Count > 0)
                return finded.ToArray();

            return null;
        }

        public RectangleShape sharedShape => shape;

        public bool isActive => active;

        public int renderLayer => layer;
    }
}
