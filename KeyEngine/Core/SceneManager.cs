namespace KeyEngine
{
    /// <summary>
    /// Static manager for loading, unloading, and controlling game scenes.
    /// </summary>
    public static class SceneManager
    {
        /// <summary>
        /// The currently active scene.
        /// </summary>
        public static IScene? CurrentScene { get; private set; }
        /// <summary>
        /// Indicates whether the current scene is actively running.
        /// </summary>
        public static bool IsSceneRunning { get; private set; }

        /// <summary>
        /// Loads a new scene of the specified type.
        /// </summary>
        /// <param name="forceGC">If true, forces garbage collection after unloading the previous scene.</param>
        public static void StartScene<T>(bool forceGC = false, bool ignoreSceneImmunity = false) where T : IScene
        {
            CurrentScene = Activator.CreateInstance<T>();
            CurrentScene.Load();
            StartScene(CurrentScene, forceGC, ignoreSceneImmunity);
        }

        /// <summary>
        /// Loads the provided scene instance.
        /// </summary>
        /// <param name="forceGC">If true, forces garbage collection after unloading the previous scene.</param>
        public static void StartScene(IScene scene, bool forceGC = false, bool ignoreSceneImmunity = false)
        {
            CurrentScene?.Unload();
            IsSceneRunning = false;

            for (int i = 0; i < ECS.EntitiesCount; i++)
            {
                Entity entity = ECS.EntityCollection[i];

                if (ECS.EntityCollection[i].SceneImmunity && !ignoreSceneImmunity)
                    continue;

                entity.Destroy();
            }

            if (forceGC)
                GC.Collect();

            CurrentScene = scene;
            CurrentScene.Load();

            IsSceneRunning = true;

            ECS.CallStart();
        }
    }
}
