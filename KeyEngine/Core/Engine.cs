namespace KeyEngine.Core
{
    public static class Engine
    {
        public static void Run(IScene startScene)
        {
            if (MainWindow.Instance != null)
                throw new InvalidOperationException("KeyEngine is already running.");

            MainWindow.Initialize();
            SceneManager.StartScene(startScene);
            MainWindow.Run();
        }
    }
}
