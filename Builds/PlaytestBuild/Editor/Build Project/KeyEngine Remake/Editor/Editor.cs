#if ENABLE_EDITOR

using KeyEngine.Editor.Systems;

namespace KeyEngine.Editor
{
    public static class Editor
    {
        private static readonly List<EditorSystem> systems = new List<EditorSystem>();

        static Editor()
        {
            RegisterSystems();
        }

        private static void RegisterSystems()
        {
            RegisterSystem<EditorGuiSystem>();
            RegisterSystem<SceneMovementSystem>();
        }

        public static void Update(float deltaTime)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                systems[i].Update(deltaTime);
            }
        }

        public static void Render()
        {
            for (int i = 0; i < systems.Count; i++)
            {
                systems[i].Render();
            }
        }

        public static void RegisterSystem<T>() where T : EditorSystem
        {
            object? instance = Activator.CreateInstance(typeof(T));

            if (instance != null)
                systems.Add((EditorSystem)instance);
        }
    }
}

#endif