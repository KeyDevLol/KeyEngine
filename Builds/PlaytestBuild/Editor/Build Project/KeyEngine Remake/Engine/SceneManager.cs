
using System.Reflection;

namespace KeyEngine
{
    public static class SceneManager
    {
        public static IScene? CurrentScene { get; private set; }
        public static bool SceneIsRunning { get; private set; }

        public static void LoadScene<T>(bool callGC = false) where T : IScene
        {
            CurrentScene?.Unload();
            SceneIsRunning = false;
            
            ECS.DeleteAllEntities();
            if (callGC)
                GC.Collect();

            CurrentScene = Activator.CreateInstance<T>();
            CurrentScene.Load();

            SceneIsRunning = true;

            ECS.CallStart();
        }
    }
}
